using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(Filters.LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
            {
                return NotFound();
            }

            if (messageFromRepo.SenderId != userId && messageFromRepo.RecipientId != userId)
            {
                return Unauthorized();
            }

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageForList>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{otherUserId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int otherUserId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messagesFromRepo = await _repo.GetMessageThread(userId, otherUserId);
            var messages = _mapper.Map<IEnumerable<MessageForList>>(messagesFromRepo);

            return Ok(messages);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreation messageForCreation)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var recipient = await _repo.GetUser(messageForCreation.RecipientId);

            if (recipient == null)
            {
                return NotFound();
            }

            var message = _mapper.Map<Message>(messageForCreation);
            message.SentDate = DateTime.UtcNow;
            message.SenderId = userId;

            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                // Load the sender detail so that automapper can use it
                var sender = await _repo.GetUser(userId);
                var messageToReturn = _mapper.Map<MessageForList>(message);
                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);
            }

            throw new Exception("Creation failed on save");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageToDelete = await _repo.GetMessage(id);

            if (messageToDelete.SenderId != userId && messageToDelete.RecipientId != userId)
            {
                return Unauthorized();
            }

            if (messageToDelete.SenderId == userId)
            {
                messageToDelete.SenderDeleted = true;
            }
            
            if (messageToDelete.RecipientId == userId)
            {
                messageToDelete.RecipientDeleted = true;
            }

            if(messageToDelete.SenderDeleted && messageToDelete.RecipientDeleted) {
                _repo.Delete(messageToDelete);
            }

            if(await _repo.SaveAll()) {
                return NoContent();
            }

            throw new Exception("Deletion failed on save");
        }
    }
}