using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingContext : DbContext
    {
        public DatingContext(DbContextOptions<DatingContext> options)
            : base(options)
        { }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}