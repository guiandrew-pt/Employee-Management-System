using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Repository.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        //Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        //Task<IEnumerable<Employee>> SearchEmployeesAsync(
        //    string? firstName,
        //    string? lastName,
        //    DateTime? dateOfBirth,
        //    int pageNumber,
        //    int pageSize);

        Task<(IEnumerable<Employee>, int)> GetAllEmployeesAsync(int page, int pageSize);
        Task<(IEnumerable<Employee>, int)> SearchEmployeesWithCountAsync(
            string? firstName, string? lastName, DateTime? dateOfBirth, int page, int pageSize);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<int> AddEmployeeAsync(Employee employee);
        Task<int> UpdateEmployeeAsync(Employee employee);
        Task<int> DeleteEmployeeAsync(int id);
    }
}

