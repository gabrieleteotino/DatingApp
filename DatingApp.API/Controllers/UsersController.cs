using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var usersVM = _mapper.Map<IEnumerable<UserForList>>(users);

            return Ok(usersVM);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userVM = _mapper.Map<UserForDetail>(user);

            return Ok(userVM);
        }

        // PUT: api/user/3
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdate userForUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // User is a ClaimsPrincipal defined in ControllerBase, we search for the first identity that has an id (NameIdentifier)
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound($"Could not find a user with an ID of {id}");
            }

            // Only the current user can change its profile
            if (userFromRepo.Id != currentUserId)
            {
                return Unauthorized();
            }

            _mapper.Map(userForUpdate, userFromRepo);

            if (await _repo.SaveAll())
            {
                // With PUT the response should be a 204 when everything is ok
                return NoContent();
            }
            else
            {
                throw new Exception($"Updating user {id} failed on save");
            }
        }
    }
}