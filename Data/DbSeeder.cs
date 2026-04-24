using CriticsDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CriticsDB.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            // Seed roles
            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin
            if (await userManager.FindByEmailAsync("admin@criticsdb.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@criticsdb.com",
                    Email = "admin@criticsdb.com",
                    DisplayName = "Administrator",
                    Bio = "CriticsDB platform administrator.",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed demo user
            if (await userManager.FindByEmailAsync("demo@criticsdb.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "demo@criticsdb.com",
                    Email = "demo@criticsdb.com",
                    DisplayName = "Demo Critic",
                    Bio = "Passionate about cinema since forever.",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Demo@123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "User");
            }

            // Seed movies
            if (!context.Movies.Any())
            {
                var movies = new List<Movie>
                {
                    new() {
                        Title = "Inception",
                        Description = "A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O., but his tragic past may doom the project and his team to disaster.",
                        Genre = "Sci-Fi / Thriller",
                        ReleaseDate = new DateTime(2010, 7, 16),
                        Director = "Christopher Nolan",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=YoHD9XEInc0"
                    },
                    new() {
                        Title = "The Dark Knight",
                        Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                        Genre = "Action / Drama",
                        ReleaseDate = new DateTime(2008, 7, 18),
                        Director = "Christopher Nolan",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=EXeTwQWrcwY"
                    },
                    new() {
                        Title = "Interstellar",
                        Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                        Genre = "Sci-Fi / Adventure",
                        ReleaseDate = new DateTime(2014, 11, 7),
                        Director = "Christopher Nolan",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=zSWdZVtXT7E"
                    },
                    new() {
                        Title = "Parasite",
                        Description = "Greed and class discrimination threaten the newly formed symbiotic relationship between the wealthy Park family and the destitute Kim clan.",
                        Genre = "Drama / Thriller",
                        ReleaseDate = new DateTime(2019, 11, 8),
                        Director = "Bong Joon-ho",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=5xH0HfJHsaY"
                    },
                    new() {
                        Title = "The Shawshank Redemption",
                        Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                        Genre = "Drama",
                        ReleaseDate = new DateTime(1994, 9, 23),
                        Director = "Frank Darabont",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/lyQBXzOQSuE59IsHyhrp0qIiPAz.jpg"
                    },
                    new() {
                        Title = "Dune: Part Two",
                        Description = "Paul Atreides unites with Chani and the Fremen while on a path of revenge against the conspirators who destroyed his family.",
                        Genre = "Sci-Fi / Epic",
                        ReleaseDate = new DateTime(2024, 3, 1),
                        Director = "Denis Villeneuve",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/1pdfLvkbY9ohJlCjQH2CZjjYVvJ.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=Way9Dexny3w"
                    },
                    new() {
                        Title = "Oppenheimer",
                        Description = "The story of American scientist J. Robert Oppenheimer and his role in the development of the atomic bomb during World War II.",
                        Genre = "Biography / Drama",
                        ReleaseDate = new DateTime(2023, 7, 21),
                        Director = "Christopher Nolan",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/8Gxv8gSFCU0XGDykEGv7zR1n2ua.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=uYPbbksJxIg"
                    },
                    new() {
                        Title = "Everything Everywhere All at Once",
                        Description = "An aging Chinese immigrant is swept up in an insane adventure in which she alone can save the world by exploring other universes connecting with the lives she could have led.",
                        Genre = "Sci-Fi / Comedy",
                        ReleaseDate = new DateTime(2022, 3, 25),
                        Director = "Daniel Kwan, Daniel Scheinert",
                        PosterUrl = "https://image.tmdb.org/t/p/w500/w3LxiVYdWWRvEVdn5RYq6jIqkb1.jpg"
                    }
                };
                context.Movies.AddRange(movies);
                await context.SaveChangesAsync();
            }
        }
    }
}
