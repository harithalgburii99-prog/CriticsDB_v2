using System.ComponentModel.DataAnnotations;

namespace CriticsDB.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Required]
        public DateTime ReleaseDate { get; set; }

        public string? PosterUrl { get; set; }

        public string? TrailerUrl { get; set; }

        [MaxLength(100)]
        public string? Director { get; set; }

        public string Type { get; set; } = "Movie"; // "Movie" or "Series"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        public double AverageRating =>
            Ratings.Any() ? Math.Round(Ratings.Average(r => r.Value), 1) : 0;

        public int RatingCount => Ratings.Count;
        public int ReviewCount => Reviews.Count;
    }
}