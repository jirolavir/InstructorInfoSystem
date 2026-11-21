
using System.ComponentModel.DataAnnotations;

namespace InstructorInfoSystem.Models
{
    public class Instructor
    {
        [Key]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Please enter the full name.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the age.")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please enter the address.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the designation.")]
        [StringLength(100, ErrorMessage = "Designation cannot exceed 100 characters.")]
        [Display(Name = "Designation")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select the gender.")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the contact number.")]
        [Phone(ErrorMessage = "Enter a valid contact number.")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the majorship.")]
        [StringLength(100, ErrorMessage = "Majorship cannot exceed 100 characters.")]
        [Display(Name = "Majorship")]
        public string Majorship { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the bachelor degree.")]
        [StringLength(150, ErrorMessage = "Bachelor degree cannot exceed 150 characters.")]
        [Display(Name = "Bachelor Degree")]
        public string BachelorDegree { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "E-mail Address")]
        public string Email { get; set; } = string.Empty;
    }
}
