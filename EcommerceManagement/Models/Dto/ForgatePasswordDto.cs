using System.ComponentModel.DataAnnotations;

namespace EcommerceManagement.Models.Dto
{
    public class ForgatePasswordDto
    { 

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords don't match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
