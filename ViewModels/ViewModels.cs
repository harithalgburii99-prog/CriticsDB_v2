using System.ComponentModel.DataAnnotations;

namespace CriticsDB.ViewModels
{
    // Auth ViewModels
    public class RegisterViewModel
    {
        [Required, MaxLength(100)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password"), DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    // Movie ViewModels
    public class MovieFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Required, Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Poster Image URL")]
        public string? PosterUrl { get; set; }

        [Display(Name = "Trailer URL")]
        public string? TrailerUrl { get; set; }

        [MaxLength(100)]
        public string? Director { get; set; }
    }

    public class MovieListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string? PosterUrl { get; set; }
        public string? Director { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public int ReviewCount { get; set; }
    }

    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public string? Director { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public int? CurrentUserRating { get; set; }
        public ReviewFormViewModel? ReviewForm { get; set; }
        public bool HasUserReviewed { get; set; }
        public int[] RatingDistribution { get; set; } = new int[10]; // index 0 = rating 1
    }

    // Review ViewModels
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public bool? CurrentUserLiked { get; set; } // null = no reaction, true = like, false = dislike
        public bool IsOwnReview { get; set; }
    }

    public class ReviewFormViewModel
    {
        public int Id { get; set; }
        public int MovieId { get; set; }

        [Required, MaxLength(2000)]
        public string Comment { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Rating { get; set; } = 5;
    }

    // Profile ViewModel
    public class UserProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime JoinedAt { get; set; }
        public int ReviewCount { get; set; }
        public int RatingCount { get; set; }
        public double AverageRatingGiven { get; set; }
        public List<RecentReviewViewModel> RecentReviews { get; set; } = new();
    }

    public class EditProfileViewModel
    {
        [Required, MaxLength(100)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Bio { get; set; }

        [Display(Name = "Avatar URL")]
        public string? AvatarUrl { get; set; }
    }

    public class RecentReviewViewModel
    {
        public int ReviewId { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string? MoviePoster { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Home ViewModel
    public class HomeViewModel
    {
        public List<MovieListViewModel> TopRated { get; set; } = new();
        public List<MovieListViewModel> Latest { get; set; } = new();
        public List<MovieListViewModel> MostReviewed { get; set; } = new();
    }

    // Movie Index / Search ViewModel
    public class MovieIndexViewModel
    {
        public List<MovieListViewModel> Movies { get; set; } = new();
        public string? SearchQuery { get; set; }
        public string? Genre { get; set; }
        public string SortBy { get; set; } = "latest";
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<string> Genres { get; set; } = new();
    }

    // Admin ViewModels
    public class AdminDashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalUsers { get; set; }
        public int TotalReviews { get; set; }
        public int TotalRatings { get; set; }
        public List<MovieListViewModel> RecentMovies { get; set; } = new();
        public List<AdminReviewViewModel> RecentReviews { get; set; } = new();
    }

    public class AdminReviewViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public int ReviewCount { get; set; }
        public int RatingCount { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsLockedOut { get; set; }
    }
}
