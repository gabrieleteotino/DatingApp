using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DatingContext _context;
        public DatingRepository(DatingContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(x => x.FromId == userId && x.ToId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.FirstOrDefaultAsync(x => x.UserId == userId && x.IsMain);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                .AsQueryable();
            
            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId);
                    break;
                default: //"Unread"
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.IsRead);
                    break;
            }

            messages = messages.OrderByDescending(m=>m.SentDate);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(x => x.LastActive).AsQueryable();
            users = users.Where(x => x.Id != userParams.UserId && x.Gender == userParams.Gender);
            if (userParams.MinAge != UserParams.MinAgeDefault || userParams.MaxAge != UserParams.MaxAgeDefault)
            {
                // Precalculate the dates so the database can optimize the query
                var today = DateTime.Today;
                var minAgeDateOfBirth = today.AddYears(-userParams.MinAge);
                var maxAgeDateOfBirth = today.AddYears(-userParams.MaxAge - 1).AddDays(1);
                users = users.Where(x => x.DateOfBirth <= minAgeDateOfBirth && x.DateOfBirth >= maxAgeDateOfBirth);
            }
            // Users that likes the current user
            if (userParams.Likers)
            {
                // The users who have in the list of liked user a like pointing to the current user
                users = users.Where(x => x.LikeTo.Any(y => y.ToId == userParams.UserId));
            }
            // Users that the current user likes
            if (userParams.Likees)
            {
                // The users who have in the list of likers a like originated from the current user
                users = users.Where(x => x.LikesFrom.Any(y => y.FromId == userParams.UserId));
            }
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(x => x.Created);
                        break;
                    default:
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}