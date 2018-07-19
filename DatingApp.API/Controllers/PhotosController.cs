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
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("faces:center")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }

                // Check if the upload was fine
                if (uploadResult.Error != null)
                {
                    return BadRequest("Unable to upload photo.\n" + uploadResult.Error.Message);
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

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            // Only the current user can set it's main photo
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != currentUserId)
            {
                return Unauthorized();
            }

            var photoToSet = await _repo.GetPhoto(id);
            if (photoToSet == null)
            {
                return NotFound($"The photo {id} does not exists.");
            }

            if (photoToSet.IsMain)
            {
                return BadRequest("This is already the main photo");
            }

            // The current user cannot set a main photo from another user 
            if (photoToSet.UserId != userId)
            {
                return Unauthorized();
            }

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            if (currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
            }

            photoToSet.IsMain = true;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set the photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // Only the current user can delete it's main photo
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != currentUserId)
            {
                return Unauthorized();
            }

            var photoToDelete = await _repo.GetPhoto(id);
            if (photoToDelete == null)
            {
                return NotFound($"The photo {id} does not exists.");
            }

            if (photoToDelete.IsMain)
            {
                return BadRequest("You cannot delete the main photo");
            }

            // The test photos are seeded without a PhotoId
            if (string.IsNullOrEmpty(photoToDelete.PublicId))
            {
                _repo.Delete(photoToDelete);
            }
            else
            {
                var result = _cloudinary.Destroy(new DeletionParams(photoToDelete.PublicId));
                if (result.Result == "ok")
                {
                    _repo.Delete(photoToDelete);
                }
            }

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Could not delete the photo");
        }
    }
}