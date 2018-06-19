using System;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatingContext _context;

        public AuthRepository(DatingContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            if(string.IsNullOrWhiteSpace(username)) return null;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username.ToLowerInvariant());

            if (user == null) return null;

            bool isPasswordValid = HmacHelper.CheckPasswordHash(password, user.PasswordHash, user.PasswordSalt);

            if (isPasswordValid)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            if(user == null) return null;
            // The password must be a not empty string
            if (string.IsNullOrWhiteSpace(password)) return null;

            (user.PasswordHash, user.PasswordSalt) = HmacHelper.ComputeHashSalt(password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            if(string.IsNullOrWhiteSpace(username)) return false;

            return await _context.Users.AnyAsync(x => x.Username == username.ToLowerInvariant());
        }
    }
}