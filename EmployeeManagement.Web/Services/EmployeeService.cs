using System.Net.Http.Json;
using EmployeeManagement.Web.DTOs;
using EmployeeManagement.Web.ViewModels;

namespace EmployeeManagement.Web.Services
{
	public class EmployeeService
	{
        private readonly HttpClient _http;

        public EmployeeService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaginatedResponse<EmployeeDTO>> GetAllEmployeesAsync(
            int pageNumber = 1, int pageSize = 10)
        {
            // Build query string with parameters
            string query = $"api/Employees/search?pageNumber={pageNumber}&pageSize={pageSize}";

            // Fetch paginated data
            PaginatedResponse<EmployeeDTO>? response = await _http.GetFromJsonAsync<PaginatedResponse<EmployeeDTO>>(query);

            return response ?? new PaginatedResponse<EmployeeDTO>(); // Return empty response if null
        }

        public async Task<PaginatedResponse<EmployeeDTO>> SearchEmployeesAsync(
            string? firstName = null,
            string? lastName = null,
            DateTime? dateOfBirth = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            // Build query string with optional parameters
            string query = $"api/Employees/search?pageNumber={pageNumber}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(firstName))
            {
                query += $"&firstName={Uri.EscapeDataString(firstName)}";
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                query += $"&lastName={Uri.EscapeDataString(lastName)}";
            }
            if (dateOfBirth.HasValue)
            {
                query += $"&dateOfBirth={Uri.EscapeDataString(dateOfBirth.Value.ToString("yyyy-MM-dd"))}";
            }

            // Make HTTP GET request with the query string
            // Fetch paginated data
            PaginatedResponse<EmployeeDTO>? response = await _http.GetFromJsonAsync<PaginatedResponse<EmployeeDTO>>(query);

            return response ?? new PaginatedResponse<EmployeeDTO>(); // Return empty response if null
        }

        public async Task<EmployeeDTO?> GetEmployeeByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<EmployeeDTO>($"api/Employees/{id}");
        }

        public async Task CreateEmployeeAsync(EmployeeDTO employee)
        {
            await _http.PostAsJsonAsync("api/Employees", employee);
        }

        public async Task UpdateEmployeeAsync(EmployeeDTO? employee)
        {
            await _http.PutAsJsonAsync($"api/Employees/{employee?.Id}", employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _http.DeleteAsync($"api/Employees/{id}");
        }
    }
}

