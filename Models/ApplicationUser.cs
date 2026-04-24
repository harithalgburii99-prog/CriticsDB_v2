using Microsoft.AspNetCore.Identity;

namespace CriticsDB.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    }
}
