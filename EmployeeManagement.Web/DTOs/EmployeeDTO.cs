using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Web.DTOs
{
    public class EmployeeDTO
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50, ErrorMessage = "First Name is too long. Cannot be longer than 50 characters.")]
        public string? FirstName { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Last Name is too long. Cannot be longer than 50 characters.")]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName => $"{FirstName} {LastName}";

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

