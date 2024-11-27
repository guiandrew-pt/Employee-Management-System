using Dapper;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Repository.Repositories;
using MySql.Data.MySqlClient;
using static Dapper.SqlMapper;
using DotNetEnv;
using EmployeeManagement.Test.Data;

namespace EmployeeManagement.Test.RepositoryTests
{
    public class EmployeeRepositoryTests
    {
        private readonly string? _testConnection;

        public EmployeeRepositoryTests()
        {
            // Load environment variables
            Env.TraversePath().Load();

            string connectionString = Env.GetString("CONNECTIONSTRINGTEST");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The test connection string is not configured!");
            }

            _testConnection = connectionString;
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsEmployee_WhenEmployeeExists_InMemory()
        {
            // Arrange

            using var connection = new MySqlConnection(_testConnection);
            await connection.OpenAsync();

            // Create an isolated in-memory table (or truncate for repeated tests)
            const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FirstName VARCHAR(50),
                    LastName VARCHAR(50),
                    Email VARCHAR(50),
                    Phone VARCHAR(20),
                    Department VARCHAR(50),
                    DateOfBirth DATE
                );
            ";
            await connection.ExecuteAsync(createTableQuery);

            const string insertQuery = @"
                INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, Department, DateOfBirth)
                VALUES (@Id, @FirstName, @LastName, @Email, @Phone, @Department, @DateOfBirth);
            ";
            await connection.ExecuteAsync(insertQuery, new
            {
                Id = 1,
                FirstName = "Milena",
                LastName = "Joew",
                Email = "milena.joew@example.com",
                Phone = "(123) 456-7890",
                Department = "Chief",
                DateOfBirth = new DateTime(1990, 1, 1)
            });

            // Use the TestMySqlConnectionFactory to ensure the repository uses the test connection
            var factory = new TestMySqlConnectionFactory(_testConnection);
            var repository = new EmployeeRepository(factory);

            // Act
            var result = await repository.GetEmployeeByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Milena", result.FirstName);

            // No cleanup is needed since we used a temporary table
        }

        [Fact]
        public async Task AddEmployeeAsync_ReturnsAffectedRows_WhenInsertSucceeds()
        {
            // Arrange
            using var connection = new MySqlConnection(_testConnection);
            await connection.OpenAsync();

            // Clear table before test
            const string clearTableQuery = "TRUNCATE TABLE Employees";
            await connection.ExecuteAsync(clearTableQuery);

            // Create table if not exists
            const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FirstName VARCHAR(50),
                    LastName VARCHAR(50),
                    Email VARCHAR(50),
                    Phone VARCHAR(20),
                    Department VARCHAR(50),
                    DateOfBirth DATE
                );
            ";
            await connection.ExecuteAsync(createTableQuery);

            // Use the TestMySqlConnectionFactory to ensure the repository uses the test connection
            var factory = new TestMySqlConnectionFactory(_testConnection);
            var repository = new EmployeeRepository(factory);

            var newEmployee = new Employee
            {
                FirstName = "Madame",
                LastName = "Smith",
                Email = "madame.smith@example.com",
                Phone = "(321) 654-0987",
                Department = "HR",
                DateOfBirth = new DateTime(1985, 5, 15)
            };

            // Act
            var insertedEmployeeId = await repository.AddEmployeeAsync(newEmployee);

            // Debug: Print all employees
            const string debugQuery = "SELECT * FROM Employees";
            var allEmployees = await connection.QueryAsync<Employee>(debugQuery);
            //Console.WriteLine("Employees in database:");
            //foreach (var emp in allEmployees)
            //{
            //    Console.WriteLine($"Id: {emp.Id}, FirstName: {emp.FirstName}, LastName: {emp.LastName}");
            //}

            // Verify the inserted employee
            const string verifyQuery = "SELECT * FROM Employees WHERE Id = @Id";
            var insertedEmployee = await connection.QuerySingleOrDefaultAsync<Employee>(verifyQuery, new { Id = insertedEmployeeId });

            // Assert
            Assert.NotNull(insertedEmployee);
            Assert.Equal(newEmployee.FirstName, insertedEmployee!.FirstName);
            Assert.Equal(newEmployee.LastName, insertedEmployee.LastName);
            Assert.Equal(newEmployee.Email, insertedEmployee.Email);
            Assert.Equal(newEmployee.Phone, insertedEmployee.Phone);
            Assert.Equal(newEmployee.Department, insertedEmployee.Department);
            Assert.Equal(newEmployee.DateOfBirth, insertedEmployee.DateOfBirth);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ReturnsAffectedRows_WhenDeleteSucceeds()
        {
            // Arrange
            const int employeeId = 1;

            // Arrange
            using var connection = new MySqlConnection(_testConnection);
            await connection.OpenAsync();

            // Clear table before test
            const string clearTableQuery = "TRUNCATE TABLE Employees";
            await connection.ExecuteAsync(clearTableQuery);

            // Create an Employees table and seed some data
            const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FirstName VARCHAR(50),
                    LastName VARCHAR(50),
                    Email VARCHAR(50),
                    Phone VARCHAR(20),
                    Department VARCHAR(50),
                    DateOfBirth DATE
                );
            ";

            await connection.ExecuteAsync(createTableQuery);

            // Seed data
            const string insertQuery = @"
                INSERT INTO Employees (FirstName, LastName, Email, Phone, Department, DateOfBirth)
                VALUES
                    ('John', 'Lee', 'john.lee@example.com', '(123) 456-7890', 'Engineering', '1990-01-01'),
                    ('Jane', 'See', 'jane.see@example.com', '(321) 654-0987', 'Marketing', '1985-05-15');
            ";
            await connection.ExecuteAsync(insertQuery);

            // Use the TestMySqlConnectionFactory to ensure the repository uses the test connection
            var factory = new TestMySqlConnectionFactory(_testConnection);
            var repository = new EmployeeRepository(factory);

            // Act
            var affectedRows = await repository.DeleteEmployeeAsync(employeeId);

            // Assert
            Assert.Equal(1, affectedRows);

            // Verify that the employee no longer exists
            var deletedEmployee = await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE Id = @Id", new { Id = employeeId });
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public async Task SearchEmployeesWithCountAsync_ReturnsCorrectResults_InMemory()
        {
            // Arrange
            using var connection = new MySqlConnection(_testConnection);
            await connection.OpenAsync();

            // Truncate table to ensure clean data
            await connection.ExecuteAsync("TRUNCATE TABLE Employees;");

            // Create table
            const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FirstName VARCHAR(50),
                    LastName VARCHAR(50),
                    Email VARCHAR(50),
                    Phone VARCHAR(20),
                    Department VARCHAR(50),
                    DateOfBirth DATE
                );
            ";
            await connection.ExecuteAsync(createTableQuery);

            // Seed data
            const string insertQuery = @"
                INSERT INTO Employees (FirstName, LastName, Email, Phone, Department, DateOfBirth)
                VALUES
                    ('John', 'Lee', 'john.lee@example.com', '(123) 456-7890', 'Engineering', '1990-01-01'),
                    ('Jane', 'See', 'jane.see@example.com', '(321) 654-0987', 'Marketing', '1985-05-15');
            ";
            await connection.ExecuteAsync(insertQuery);

            // Use the TestMySqlConnectionFactory to ensure the repository uses the test connection
            var factory = new TestMySqlConnectionFactory(_testConnection);
            var repository = new EmployeeRepository(factory);

            // Act
            var (employees, count) = await repository.SearchEmployeesWithCountAsync(
                firstName: "John",
                lastName: null,
                dateOfBirth: null,
                page: 1,
                pageSize: 10);

            // Assert
            Assert.Equal(1, count); // Only one employee matches "John"
            var employee = Assert.Single(employees); // Ensures one record is returned on the current page
            Assert.Equal("John", employee.FirstName);
            Assert.Equal("Lee", employee.LastName);
        }

        [Fact]
        public async Task SearchEmployeesWithCountAsync_NoFilters_ReturnsAllEmployees()
        {
            // Arrange
            // Use the TestMySqlConnectionFactory to ensure the repository uses the test connection
            var factory = new TestMySqlConnectionFactory(_testConnection);
            var repository = new EmployeeRepository(factory);
            var (employees, count) = await repository.SearchEmployeesWithCountAsync(
                firstName: null,
                lastName: null,
                dateOfBirth: null,
                page: 1,
                pageSize: 10);

            Assert.Equal(2, count); // Total seeded employees
            Assert.Equal(2, employees.Count()); // Both records should be included on the first page
        }
    }
}

