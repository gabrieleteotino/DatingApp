using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DatingContext _context;

        public Seed(DatingContext context)
        {
            _context = context;
        }

        public async Task SeedUsers()
        {
            // If the db is not empty abort the seeding
            if(await _context.Users.AnyAsync()) {
                return;
            }

            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            // All the test user will use the password "password"
            (byte[] passwordHash, byte[] passwordSalt) = Helpers.HmacHelper.ComputeHashSalt("password");

            foreach (var user in users)
            {
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                user.Username = user.Username.ToLowerInvariant();

                _context.Users.Add(user);
            }

            await _context.SaveChangesAsync();
        }
    }
}