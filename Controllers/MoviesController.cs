using CriticsDB.Data;
using CriticsDB.Models;
using CriticsDB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CriticsDB.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int PageSize = 12;

        public MoviesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, string? genre, string sortBy = "latest", int page = 1)
        {
            var query = _context.Movies
                .Include(m => m.Ratings)
                .Include(m => m.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Title.Contains(search) || m.Director!.Contains(search) || m.Genre.Contains(search));

            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(m => m.Genre.Contains(genre));

            // Sort BEFORE pagination so ordering applies across all pages
            query = sortBy switch
            {
                "top" => query.OrderByDescending(m => m.Ratings.Average(r => (double?)r.Value) ?? 0),
                "most-reviewed" => query.OrderByDescending(m => m.Reviews.Count),
                "oldest" => query.OrderBy(m => m.ReleaseDate),
                _ => query.OrderByDescending(m => m.ReleaseDate)
            };

            var genres = await _context.Movies.Select(m => m.Genre).Distinct().OrderBy(g => g).ToListAsync();
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var movies = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            var vm = new MovieIndexViewModel
            {
                Movies = movies.Select(m => new MovieListViewModel
                {
                    Id = m.Id, Title = m.Title, Genre = m.Genre, ReleaseDate = m.ReleaseDate,
                    PosterUrl = m.PosterUrl, Director = m.Director,
                    AverageRating = m.AverageRating, RatingCount = m.RatingCount, ReviewCount = m.ReviewCount
                }).ToList(),
                SearchQuery = search,
                Genre = genre,
                SortBy = sortBy,
                Page = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                Genres = genres
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Ratings)
                .Include(m => m.Reviews)
                    .ThenInclude(r => r.User)
                .Include(m => m.Reviews)
                    .ThenInclude(r => r.Likes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var userRating = userId != null ? movie.Ratings.FirstOrDefault(r => r.UserId == userId) : null;
            var userReview = userId != null ? movie.Reviews.FirstOrDefault(r => r.UserId == userId) : null;

            // Rating distribution
            var dist = new int[10];
            foreach (var r in movie.Ratings)
                dist[r.Value - 1]++;

            var vm = new MovieDetailsViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                ReleaseDate = movie.ReleaseDate,
                PosterUrl = movie.PosterUrl,
                TrailerUrl = movie.TrailerUrl,
                Director = movie.Director,
                AverageRating = movie.AverageRating,
                RatingCount = movie.RatingCount,
                CurrentUserRating = userRating?.Value,
                HasUserReviewed = userReview != null,
                RatingDistribution = dist,
                ReviewForm = new ReviewFormViewModel
                {
                    MovieId = id,
                    Id = userReview?.Id ?? 0,
                    Comment = userReview?.Comment ?? "",
                    Rating = userReview?.Rating ?? 5
                },
                Reviews = movie.Reviews.OrderByDescending(r => r.CreatedAt).Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User.DisplayName,
                    UserAvatar = r.User.AvatarUrl,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    LikeCount = r.LikeCount,
                    DislikeCount = r.DislikeCount,
                    CurrentUserLiked = userId != null ? r.Likes.FirstOrDefault(l => l.UserId == userId)?.IsLike : null,
                    IsOwnReview = r.UserId == userId
                }).ToList()
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(int movieId, int value)
        {
            if (value < 1 || value > 10) return BadRequest();
            var userId = _userManager.GetUserId(User)!;

            var existing = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
            if (existing != null)
            {
                existing.Value = value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Ratings.Add(new Rating { UserId = userId, MovieId = movieId, Value = value });
            }

            await _context.SaveChangesAsync();

            var movie = await _context.Movies.Include(m => m.Ratings).FirstAsync(m => m.Id == movieId);
            return Json(new { success = true, average = movie.AverageRating, count = movie.RatingCount });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(ReviewFormViewModel form)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", new { id = form.MovieId });

            var userId = _userManager.GetUserId(User)!;

            if (form.Id > 0)
            {
                var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == form.Id && r.UserId == userId);
                if (review != null)
                {
                    review.Comment = form.Comment;
                    review.Rating = form.Rating;
                    review.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                var exists = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.MovieId == form.MovieId);
                if (!exists)
                {
                    _context.Reviews.Add(new Review
                    {
                        UserId = userId,
                        MovieId = form.MovieId,
                        Comment = form.Comment,
                        Rating = form.Rating
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Review saved!";
            return RedirectToAction("Details", new { id = form.MovieId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            if (review.UserId != userId && !isAdmin) return Forbid();

            var movieId = review.MovieId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Review deleted.";
            return RedirectToAction("Details", new { id = movieId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeReview(int reviewId, bool isLike)
        {
            var userId = _userManager.GetUserId(User)!;
            var existing = await _context.ReviewLikes.FirstOrDefaultAsync(l => l.UserId == userId && l.ReviewId == reviewId);

            if (existing != null)
            {
                if (existing.IsLike == isLike)
                    _context.ReviewLikes.Remove(existing);
                else
                    existing.IsLike = isLike;
            }
            else
            {
                _context.ReviewLikes.Add(new ReviewLike { UserId = userId, ReviewId = reviewId, IsLike = isLike });
            }

            await _context.SaveChangesAsync();

            var review = await _context.Reviews.Include(r => r.Likes).FirstAsync(r => r.Id == reviewId);
            return Json(new { likes = review.LikeCount, dislikes = review.DislikeCount });
        }
    }
}
