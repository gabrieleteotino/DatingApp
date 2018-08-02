using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegistration userForRegistration)
        {
            // Business Validation
            if (await _repo.UserExists(userForRegistration.Username))
            {
                ModelState.AddModelError("Username", "Username already in use.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToCreate = _mapper.Map<User>(userForRegistration);
            var createdUser = await _repo.Register(userToCreate, userForRegistration.Password);

            var userToReturn = _mapper.Map<UserForDetail>(createdUser);
            return CreatedAtRoute("GetUser", new { id = userToReturn.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLogin userForLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _repo.Login(userForLogin.Username, userForLogin.Password);

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

            var userForList = _mapper.Map<UserForList>(user);

            return Ok(new { tokenString, user = userForList });
        }
    }
}