using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ATCalumni1.Models.Entities
{
    [Index(nameof(Email), IsUnique = true)]

    public class Users
    {
        [Key]  
        public int UserID { get; set; } 
        [Required(ErrorMessage = "First name required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }                
        public string? CurrentJobTitle { get; set; }            
        public string? Organization { get; set; }               
        public int? YearOfGraduation { get; set; }              
        public string? SemesterOfGraduation { get; set; }        
        public string? GitHubProfile { get; set; }               
        public string? LinkedInProfile { get; set; }             
        public byte[]? ProfileImage { get; set; }
        [Required(ErrorMessage = "Please select one.")]
        public bool IsMentor { get; set; }                     
        [Required(ErrorMessage = "Please select one.")]
        public bool IsAdmin { get; set; }                       
        [Required(ErrorMessage = "Please select one.")]
        public bool IsStudent { get; set; }                      
    }
}