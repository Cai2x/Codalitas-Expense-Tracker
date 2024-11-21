using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Old Password is required.")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New Password is required.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
    }
}
