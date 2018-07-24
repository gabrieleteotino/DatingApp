using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using static DatingApp.API.Controllers.UsersController;

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

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.FirstOrDefaultAsync(x => x.UserId == userId && x.IsMain);
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
            var users = _context.Users.Include(p => p.Photos).AsQueryable();
            users = users.Where(x => x.Id != userParams.UserId && x.Gender == userParams.Gender);
            if (userParams.MinAge != UserParams.MinAgeDefault || userParams.MaxAge != UserParams.MaxAgeDefault)
            {
                // To optimize the query we must precalculate the dates
                var today = DateTime.Today;
                var minAgeDateOfBirth = today.AddYears(-userParams.MinAge);
                var maxAgeDateOfBirth = today.AddYears(-userParams.MaxAge - 1).AddDays(1);
                // users = users.Where(x=>x.DateOfBirth.CalculateAge() >= userParams.MinAge && x.DateOfBirth.CalculateAge() <= userParams.MaxAge);                
                users = users.Where(x => x.DateOfBirth <= minAgeDateOfBirth && x.DateOfBirth >= maxAgeDateOfBirth);
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}