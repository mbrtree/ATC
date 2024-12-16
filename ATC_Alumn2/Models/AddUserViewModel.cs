using System.ComponentModel.DataAnnotations;

namespace ATC_Alumn2.Models
{
    public class AddUserViewModel
    {
        [Required(ErrorMessage = "First name required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email required.")]
        [EmailAddress(ErrorMessage = "Please enter valid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Phone number must be in the format 555-555-5555.")]
        public string PhoneNumber { get; set; }
        public string? CurrentJobTitle { get; set; }
        public string? Organization { get; set; }
        public int? YearOfGraduation { get; set; }
        public string? SemesterOfGraduation { get; set; }
        public string? GitHubProfile { get; set; }
        public string? LinkedInProfile { get; set; }
        public IFormFile? ProfileImage { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; } 
    }
}
