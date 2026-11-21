using System.ComponentModel.DataAnnotations;

namespace InstructorInfoSystem.ViewModels
{
    public class DeleteAccountViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [Compare("ConfirmationText",
            ErrorMessage = "Type DELETE to confirm.")]
        public string ConfirmationText { get; set; } = string.Empty;

        [Required]
        public string ConfirmationTextExpected { get; set; } = "DELETE";
    }
}