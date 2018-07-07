using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings;
            _mapper = mapper;
            _repo = repo;

            // Initialize Cloudinary
            Account account = new Account(
                cloudinarySettings.Value.CloudName,
                cloudinarySettings.Value.ApiKey,
                cloudinarySettings.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photoForReturn = _mapper.Map<PhotoForReturn>(photoFromRepo);

            return Ok(photoForReturn);
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreation photoDto)
        {
            var user = await _repo.GetUser(userId);

            if (user == null)
            {
                return BadRequest("Could not find user");
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Only the current user can upload it's photos
            if (user.Id != currentUserId)
            {
                return Unauthorized();
            }

            var file = photoDto.File;

            if (file.Length > 0)
            {
                var uploadResult = new ImageUploadResult();

                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }

                // Save the results back in the DTO
                photoDto.Url = uploadResult.Uri.ToString();
                photoDto.PublicId = uploadResult.PublicId;

                // Map the dto to a Photo model
                var photo = _mapper.Map<Photo>(photoDto);
                photo.User = user;

                // Check if the photo has to be the main photo for the user
                if (!user.Photos.Any(x => x.IsMain))
                {
                    photo.IsMain = true;
                }

                user.Photos.Add(photo);

                if (await _repo.SaveAll())
                {
                    var photoForReturn = _mapper.Map<PhotoForReturn>(photo);
                    return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoForReturn);
                }
            }
            return BadRequest("Could not add the photo");
        }
    }
}