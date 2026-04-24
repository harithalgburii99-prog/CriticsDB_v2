using CriticsDB.Data;
using CriticsDB.Models;
using CriticsDB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CriticsDB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.Include(m => m.Ratings).Include(m => m.Reviews).ToListAsync();
            var reviews = await _context.Reviews.Include(r => r.User).Include(r => r.Movie).OrderByDescending(r => r.CreatedAt).Take(10).ToListAsync();

            var vm = new AdminDashboardViewModel
            {
                TotalMovies = await _context.Movies.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalRatings = await _context.Ratings.CountAsync(),
                RecentMovies = movies.OrderByDescending(m => m.CreatedAt).Take(5).Select(m => new MovieListViewModel
                {
                    Id = m.Id, Title = m.Title, Genre = m.Genre, ReleaseDate = m.ReleaseDate,
                    PosterUrl = m.PosterUrl, AverageRating = m.AverageRating, RatingCount = m.RatingCount, ReviewCount = m.ReviewCount
                }).ToList(),
                RecentReviews = reviews.Select(r => new AdminReviewViewModel
                {
                    Id = r.Id, UserName = r.User.DisplayName, MovieTitle = r.Movie.Title,
                    MovieId = r.MovieId, Comment = r.Comment, Rating = r.Rating, CreatedAt = r.CreatedAt
                }).ToList()
            };

            return View(vm);
        }

        // Movies CRUD
        public async Task<IActionResult> Movies(string? search, int page = 1)
        {
            const int pageSize = 20;
            var query = _context.Movies.Include(m => m.Ratings).Include(m => m.Reviews).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Title.Contains(search));

            var total = await query.CountAsync();
            var movies = await query.OrderByDescending(m => m.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            return View(movies.Select(m => new MovieListViewModel
            {
                Id = m.Id, Title = m.Title, Genre = m.Genre, ReleaseDate = m.ReleaseDate,
                PosterUrl = m.PosterUrl, AverageRating = m.AverageRating, RatingCount = m.RatingCount, ReviewCount = m.ReviewCount
            }).ToList());
        }

        [HttpGet]
        public IActionResult CreateMovie() => View(new MovieFormViewModel { ReleaseDate = DateTime.Today });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMovie(MovieFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            _context.Movies.Add(new Movie
            {
                Title = vm.Title, Description = vm.Description, Genre = vm.Genre,
                ReleaseDate = vm.ReleaseDate, PosterUrl = vm.PosterUrl, TrailerUrl = vm.TrailerUrl, Director = vm.Director
            });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Movie added!";
            return RedirectToAction("Movies");
        }

        [HttpGet]
        public async Task<IActionResult> EditMovie(int id)
        {
            var m = await _context.Movies.FindAsync(id);
            if (m == null) return NotFound();
            return View(new MovieFormViewModel
            {
                Id = m.Id, Title = m.Title, Description = m.Description, Genre = m.Genre,
                ReleaseDate = m.ReleaseDate, PosterUrl = m.PosterUrl, TrailerUrl = m.TrailerUrl, Director = m.Director
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(MovieFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var m = await _context.Movies.FindAsync(vm.Id);
            if (m == null) return NotFound();
            m.Title = vm.Title; m.Description = vm.Description; m.Genre = vm.Genre;
            m.ReleaseDate = vm.ReleaseDate; m.PosterUrl = vm.PosterUrl; m.TrailerUrl = vm.TrailerUrl; m.Director = vm.Director;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Movie updated!";
            return RedirectToAction("Movies");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var m = await _context.Movies.FindAsync(id);
            if (m != null) { _context.Movies.Remove(m); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Movie deleted.";
            return RedirectToAction("Movies");
        }

        // Reviews management
        public async Task<IActionResult> Reviews(int page = 1)
        {
            const int pageSize = 20;
            var total = await _context.Reviews.CountAsync();
            var reviews = await _context.Reviews
                .Include(r => r.User).Include(r => r.Movie)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            return View(reviews.Select(r => new AdminReviewViewModel
            {
                Id = r.Id, UserName = r.User.DisplayName, MovieTitle = r.Movie.Title,
                MovieId = r.MovieId, Comment = r.Comment, Rating = r.Rating, CreatedAt = r.CreatedAt
            }).ToList());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var r = await _context.Reviews.FindAsync(id);
            if (r != null) { _context.Reviews.Remove(r); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Review deleted.";
            return RedirectToAction("Reviews");
        }

        // Users management
        public async Task<IActionResult> Users(int page = 1)
        {
            const int pageSize = 20;
            var users = await _userManager.Users.OrderBy(u => u.JoinedAt).ToListAsync();
            var vms = new List<AdminUserViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                vms.Add(new AdminUserViewModel
                {
                    Id = u.Id, DisplayName = u.DisplayName, Email = u.Email ?? "",
                    JoinedAt = u.JoinedAt,
                    ReviewCount = await _context.Reviews.CountAsync(r => r.UserId == u.Id),
                    RatingCount = await _context.Ratings.CountAsync(r => r.UserId == u.Id),
                    Role = roles.FirstOrDefault() ?? "User",
                    IsLockedOut = await _userManager.IsLockedOutAsync(u)
                });
            }
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(vms.Count / (double)pageSize);
            return View(vms.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            TempData["Success"] = "User role updated.";
            return RedirectToAction("Users");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && user.Email != "admin@criticsdb.com")
            {
                await _userManager.DeleteAsync(user);
                TempData["Success"] = "User deleted.";
            }
            return RedirectToAction("Users");
        }
    }
}
