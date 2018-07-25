using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(Filters.LogUserActivity))]
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
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);
            if (userFromRepo == null)
            {
                return NotFound($"Could not find a user with an ID of {currentUserId}");
            }

            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _repo.GetUsers(userParams);
            var usersVM = _mapper.Map<IEnumerable<UserForList>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersVM);
        }

        [HttpGet("{id}", Name = "GetUser")]
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

        public class UserParams
        {
            private const int MaxPageSize = 50;
            public static readonly int MinAgeDefault = 18;
            public static readonly int MaxAgeDefault = 99;
            public int PageNumber { get; set; } = 1;

            private int pageSize = 10;
            public int PageSize
            {
                get { return pageSize; }
                set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
            }
            public int UserId { get; set; }
            public string Gender { get; set; }
            public int MinAge { get; set; } = MinAgeDefault;
            public int MaxAge { get; set; } = MaxAgeDefault;

            public string OrderBy { get; set; }
        }
    }
}