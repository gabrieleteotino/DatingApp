using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegistration
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 30 characters.")]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        public DateTime Created { get; }
        
        public DateTime LastActive { get; }

        public UserForRegistration()
        {
            this.Created = DateTime.UtcNow;
            this.LastActive = DateTime.UtcNow;
        }
    }
}