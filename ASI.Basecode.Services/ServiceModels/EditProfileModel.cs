using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class EditProfileModel
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        public string ProfileUrl { get; set; }
        public string Email { get; set; }
    }
}