using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Domain.Models
{
	public class Employee
	{
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50, ErrorMessage = "First Name is too long. Cannot be longer than 50 characters.")]
        public string? FirstName { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Last Name is too long. Cannot be longer than 50 characters.")]
        public string? LastName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^\([\d]{3}\) [\d]{3}-[\d]{4}$", ErrorMessage = "Phone number must be in the format (123) 456-7890")]
        public string? Phone { get; set; }

        [Required]
        public string? Department { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfBirth { get; set; }
    }
}

