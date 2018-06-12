using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]

    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegistration userForRegistration)
        {
            // TODO validation

            // Convert username to lowercase to avoid multiple user with similar names like "John" and "john"
            // Use invariant to avoid conflicts for users from different cultures
            userForRegistration.Username = userForRegistration.Username.ToLowerInvariant();

            if(await _repo.UserExists(userForRegistration.Username)){
                return BadRequest("Username already in use");
            }

            var userToCreate = new Models.User{ Username = userForRegistration.Username };
            var createdUser = await _repo.Register(userToCreate, userForRegistration.Password);

            // TODO this 201 is just a temporary solution, we should return a path to the new entity
            return StatusCode(201);
        }
    }
}