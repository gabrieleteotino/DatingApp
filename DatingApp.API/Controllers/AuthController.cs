using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]

    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegistration userForRegistration)
        {
            // Convert username to lowercase to avoid multiple user with similar names like "John" and "john"
            // Use invariant to avoid conflicts for users from different cultures
            userForRegistration.Username = userForRegistration.Username.ToLowerInvariant();

            // Business Validation
            if (await _repo.UserExists(userForRegistration.Username))
            {
                ModelState.AddModelError("Username", "Username already in use.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToCreate = new Models.User { Username = userForRegistration.Username };
            var createdUser = await _repo.Register(userToCreate, userForRegistration.Password);

            // TODO this 201 is just a temporary solution, we should return a path to the new entity
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOs.UserForLogin userForLogin)
        {
            var user = await _repo.Login(userForLogin.Username.ToLower(), userForLogin.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var key = System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:TokenSecret").Value);

            // Generate a token for the user
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });
        }
    }
}