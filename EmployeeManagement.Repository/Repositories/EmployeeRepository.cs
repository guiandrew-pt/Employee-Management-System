using EmployeeManagement.Repository.Repositories.Interfaces;
using EmployeeManagement.Domain.Models;
using Dapper;
using System.Data;
using static Dapper.SqlMapper;
using EmployeeManagement.Repository.Data;

namespace EmployeeManagement.Repository.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly MySqlConnectionFactory _connectionFactory;

        public EmployeeRepository(MySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<(IEnumerable<Employee>, int)> GetAllEmployeesAsync(int page, int pageSize)
        {
            string query = @"
                SELECT
                    Id, FirstName, LastName, Email, Phone, Department, DateOfBirth
                FROM Employees
                    WHERE (@FirstName IS NULL OR FirstName LIKE CONCAT('%', @FirstName, '%'))
                      AND (@LastName IS NULL OR LastName LIKE CONCAT('%', @LastName, '%'))
                      AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                    LIMIT @Offset, @PageSize;
            ";

            using IDbConnection connection = _connectionFactory.CreateConnection();

            using GridReader? multi = await connection.QueryMultipleAsync(query, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            IEnumerable<Employee> employees = await multi.ReadAsync<Employee>();
            int totalRecords = await multi.ReadSingleAsync<int>();

            return (employees, totalRecords);
        }

        //public async Task<IEnumerable<Employee>> SearchEmployeesAsync(
        //    string? firstName,
        //    string? lastName,
        //    DateTime? dateOfBirth,
        //    int pageNumber,
        //    int pageSize)
        //{
        //    string query = @"
        //        SELECT * FROM Employees
        //        WHERE (@FirstName IS NULL OR FirstName LIKE CONCAT('%', @FirstName, '%'))
        //          AND (@LastName IS NULL OR LastName LIKE CONCAT('%', @LastName, '%'))
        //          AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
        //        LIMIT @Offset, @PageSize";

        //    using IDbConnection connection = _connectionFactory.CreateConnection();

        //    return await connection.QueryAsync<Employee>(query, new
        //    {
        //        FirstName = firstName,
        //        LastName = lastName,
        //        DateOfBirth = dateOfBirth,
        //        Offset = (pageNumber - 1) * pageSize,
        //        PageSize = pageSize
        //    });
        //}

        public async Task<(IEnumerable<Employee>, int)> SearchEmployeesWithCountAsync(
            string? firstName, string? lastName, DateTime? dateOfBirth, int page, int pageSize)
        {
            string query = @"
                SELECT
                    Id, FirstName, LastName, Email, Phone, Department, DateOfBirth
                FROM Employees
                    WHERE (@FirstName IS NULL OR FirstName LIKE CONCAT('%', @FirstName, '%'))
                      AND (@LastName IS NULL OR LastName LIKE CONCAT('%', @LastName, '%'))
                      AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                    LIMIT @Offset, @PageSize;

                SELECT COUNT(*) FROM Employees
                WHERE (@FirstName IS NULL OR FirstName LIKE CONCAT('%', @FirstName, '%'))
                  AND (@LastName IS NULL OR LastName LIKE CONCAT('%', @LastName, '%'))
                  AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth);
            ";

            using IDbConnection connection = _connectionFactory.CreateConnection();

            using GridReader? multi = await connection.QueryMultipleAsync(query, new
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            IEnumerable<Employee> employees = await multi.ReadAsync<Employee>();
            int totalRecords = await multi.ReadSingleAsync<int>();

            return (employees, totalRecords);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                const string query = "SELECT * FROM Employees WHERE Id = @Id";

                using IDbConnection connection = _connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Id = id });
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while fetching the employee.", ex);
            } 
        }

        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            const string query = @"
                INSERT INTO Employees (FirstName, LastName, Email, Phone, Department, DateOfBirth)
                VALUES (@FirstName, @LastName, @Email, @Phone, @Department, @DateOfBirth)";

            using IDbConnection connection = _connectionFactory.CreateConnection();

            return await connection.ExecuteAsync(query, employee);
        }

        public async Task<int> UpdateEmployeeAsync(Employee employee)
        {
            const string query = @"
                UPDATE Employees
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email,
                    Phone = @Phone, Department = @Department, DateOfBirth = @DateOfBirth
                WHERE Id = @Id";

            using IDbConnection connection = _connectionFactory.CreateConnection();

            return await connection.ExecuteAsync(query, employee);
        }

        public async Task<int> DeleteEmployeeAsync(int id)
        {
            const string query = "DELETE FROM Employees WHERE Id = @Id";

            using IDbConnection connection = _connectionFactory.CreateConnection();

            return await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}

