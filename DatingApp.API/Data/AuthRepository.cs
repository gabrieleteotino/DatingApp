using System;
using System.Threading.Tasks;
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            bool isPasswordValid = CheckPasswordHash(password, user.PasswordHash, user.PasswordSalt);

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
            // The password must be a not empty string
            if (string.IsNullOrWhiteSpace(password)) return null;

            (user.PasswordHash, user.PasswordSalt) = ComputeHashSalt(password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        private (byte[] passwordHash, byte[] passwordSalt) ComputeHashSalt(string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                byte[] passwordSalt = hmac.Key;
                byte[] passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return (passwordHash: passwordHash, passwordSalt: passwordSalt);
            }
        }

        private bool CheckPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                if (passwordHash.Length != computedHash.Length) return false;

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if(passwordHash[i] != computedHash[i]) return false;
                }

                return true;
            }
        }
    }
}