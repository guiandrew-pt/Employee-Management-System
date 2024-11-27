using EmployeeManagement.API.ViewModels;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowBlazorApp")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;

        public EmployeesController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            (IEnumerable<Employee> employees, int totalRecords) = await _repository.GetAllEmployeesAsync(pageNumber, pageSize);

            PaginatedResponse<Employee>? response = new PaginatedResponse<Employee>
            {
                Data = employees.ToList(),
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] DateTime? dateOfBirth = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            // Fetch employees with optional filters and total count
            (IEnumerable<Employee> employees, int totalRecords) = await _repository.SearchEmployeesWithCountAsync(firstName, lastName, dateOfBirth, pageNumber, pageSize);

            PaginatedResponse<Employee>? response = new PaginatedResponse<Employee>
            {
                Data = employees.ToList(),
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves an employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Employee? employee = await _repository.GetEmployeeByIdAsync(id);

            if (employee is null)
                return NotFound();

            return Ok(employee);
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="employee">The employee object to create.</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            await _repository.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// Updates an existing employee.
        /// </summary>
        /// <param name="id">The ID of the employee to update.</param>
        /// <param name="employee">The updated employee object.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
                return BadRequest();

            await _repository.UpdateEmployeeAsync(employee);
            return NoContent();
        }

        /// <summary>
        /// Deletes an employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteEmployeeAsync(id);
            return NoContent();
        }
    }
}
