using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForLogin
    {
        [Required]
        public Username Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}