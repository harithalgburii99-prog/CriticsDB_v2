using System.ComponentModel.DataAnnotations;

namespace CriticsDB.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Required, MaxLength(2000)]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ReviewLike> Likes { get; set; } = new List<ReviewLike>();
        public int LikeCount => Likes.Count(l => l.IsLike);
        public int DislikeCount => Likes.Count(l => !l.IsLike);
    }

    public class Rating
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Range(1, 10)]
        public int Value { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class ReviewLike
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ReviewId { get; set; }
        public Review Review { get; set; } = null!;

        public bool IsLike { get; set; } // true = like, false = dislike
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
