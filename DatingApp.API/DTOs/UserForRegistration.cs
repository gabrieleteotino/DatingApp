using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegistration
    {
        [Required]
        public Username Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 30 characters.")]
        public string Password { get; set; }
    }
}