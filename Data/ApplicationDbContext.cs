using CriticsDB.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CriticsDB.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Unique constraint: one rating per user per movie
            builder.Entity<Rating>()
                .HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique();

            // Unique constraint: one like/dislike per user per review
            builder.Entity<ReviewLike>()
                .HasIndex(l => new { l.UserId, l.ReviewId })
                .IsUnique();

            // Unique constraint: one review per user per movie
            builder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique();

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReviewLike>()
                .HasOne(l => l.User)
                .WithMany(u => u.ReviewLikes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ReviewLike>()
                .HasOne(l => l.Review)
                .WithMany(r => r.Likes)
                .HasForeignKey(l => l.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
