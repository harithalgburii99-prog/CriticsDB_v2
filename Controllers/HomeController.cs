using CriticsDB.Data;
using CriticsDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CriticsDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.Ratings)
                .Include(m => m.Reviews)
                .ToListAsync();

            var vm = new HomeViewModel
            {
                TopRated = movies
                    .Where(m => m.Ratings.Any())
                    .OrderByDescending(m => m.AverageRating)
                    .Take(6)
                    .Select(MapToListVM)
                    .ToList(),

                Latest = movies
                    .OrderByDescending(m => m.ReleaseDate)
                    .Take(6)
                    .Select(MapToListVM)
                    .ToList(),

                MostReviewed = movies
                    .OrderByDescending(m => m.Reviews.Count)
                    .Take(6)
                    .Select(MapToListVM)
                    .ToList()
            };

            return View(vm);
        }

        private static MovieListViewModel MapToListVM(Models.Movie m) => new()
        {
            Id = m.Id,
            Title = m.Title,
            Genre = m.Genre,
            ReleaseDate = m.ReleaseDate,
            PosterUrl = m.PosterUrl,
            Director = m.Director,
            AverageRating = m.AverageRating,
            RatingCount = m.RatingCount,
            ReviewCount = m.ReviewCount
        };
    }
}
