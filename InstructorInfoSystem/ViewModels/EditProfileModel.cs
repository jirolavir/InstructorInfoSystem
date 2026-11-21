using System.ComponentModel.DataAnnotations;

namespace InstructorInfoSystem.ViewModels
{
    public class EditProfileViewModel
    {

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(1, 120, ErrorMessage = "Please enter an age.")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Please enter your address.")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your institute.")]
        [StringLength(100)]
        public string Institute { get; set; } = string.Empty;
    }
}