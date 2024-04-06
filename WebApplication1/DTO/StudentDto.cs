using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class StudentDto
    {
        public Guid? StudentId { get; set; }

        public string? ImageData { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        public string? Lastname { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Age must be a positive number")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(09|\+639)\d{9}$", ErrorMessage = "Invalid Philippine phone number format.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("Male|Female", ErrorMessage = "Gender should be either Male or Female")]
        public string? Gender { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("Regular|Irregular", ErrorMessage = "Status should be either Regular or Irregular")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Working status is required")]
        public bool Working { get; set; }

        [Required(ErrorMessage = "Student Image is required")]
        public IFormFile? File { get; set; }
    }
}
