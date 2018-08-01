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
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define a primary key for Like
            modelBuilder.Entity<Like>()
                .HasKey(x => new { x.FromId, x.ToId });

            modelBuilder.Entity<Like>()
                .HasOne(l => l.From)
                .WithMany(u => u.LikeTo)
                .HasForeignKey(l => l.FromId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.To)
                .WithMany(u => u.LikesFrom)
                .HasForeignKey(l => l.ToId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}