using CriticsDB.Data;
using CriticsDB.Models;
using CriticsDB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CriticsDB.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.Movie)
                .Where(r => r.UserId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var ratings = await _context.Ratings.Where(r => r.UserId == id).ToListAsync();

            var vm = new UserProfileViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email ?? "",
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                JoinedAt = user.JoinedAt,
                ReviewCount = reviews.Count,
                RatingCount = ratings.Count,
                AverageRatingGiven = ratings.Any() ? Math.Round(ratings.Average(r => r.Value), 1) : 0,
                RecentReviews = reviews.Take(10).Select(r => new RecentReviewViewModel
                {
                    ReviewId = r.Id,
                    MovieId = r.MovieId,
                    MovieTitle = r.Movie.Title,
                    MoviePoster = r.Movie.PosterUrl,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };

            return View(vm);
        }
    }
}
