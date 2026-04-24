# 🎬 CriticsDB — Movie Review Platform

A full-stack ASP.NET Core 8 web application for browsing, rating, and reviewing movies.

---

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server LocalDB (included with Visual Studio)
- Visual Studio 2022 **or** VS Code with C# extension

---

### 1. Clone / Extract
```bash
# Extract the ZIP then navigate into the folder
cd CriticsDB
```

### 2. Configure the Database Connection
Edit `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CriticsDB;Trusted_Connection=True;"
}
```

For a full SQL Server instance:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=CriticsDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

### 3. Restore Packages
```bash
dotnet restore
```

### 4. Apply Migrations & Seed the Database
The database is created and seeded automatically on first run. Or run manually:
```bash
dotnet ef database update
```

### 5. Run the App
```bash
dotnet run
```

Then open: **https://localhost:5001** (or the port shown in terminal)

---

## 🔑 Default Credentials

| Role  | Email                   | Password    |
|-------|-------------------------|-------------|
| Admin | admin@criticsdb.com     | Admin@123!  |
| User  | demo@criticsdb.com      | Demo@123!   |

---

## 📁 Project Structure

```
CriticsDB/
├── Controllers/
│   ├── HomeController.cs          # Home page (top rated, latest, most reviewed)
│   ├── MoviesController.cs        # Movie list, details, rate, review, like
│   ├── AccountController.cs       # Login, register, logout, edit profile
│   ├── ProfileController.cs       # User profile page
│   └── AdminController.cs         # Admin dashboard, CRUD movies/reviews/users
│
├── Models/
│   ├── ApplicationUser.cs         # Extended Identity user
│   ├── Movie.cs                   # Movie entity
│   └── Review.cs                  # Review, Rating, ReviewLike entities
│
├── ViewModels/
│   └── ViewModels.cs              # All ViewModels (Auth, Movie, Review, Admin, Profile)
│
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── DbSeeder.cs                # Seeds roles, users, and sample movies
│
├── Migrations/                    # EF Core migrations
│
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Main layout (navbar, footer, toasts)
│   │   ├── _MovieCard.cshtml      # Reusable movie card partial
│   │   └── _MovieSection.cshtml   # Reusable movie grid section
│   ├── Home/
│   │   └── Index.cshtml           # Hero + Top Rated / Latest / Most Reviewed
│   ├── Movies/
│   │   ├── Index.cshtml           # Movie list with search, filter, sort, pagination
│   │   └── Details.cshtml         # Movie details, star rater, reviews, like/dislike
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── EditProfile.cshtml
│   ├── Profile/
│   │   └── Details.cshtml         # User profile with stats and recent reviews
│   └── Admin/
│       ├── Index.cshtml           # Dashboard with stats and recent activity
│       ├── Movies.cshtml          # Movie management table
│       ├── CreateMovie.cshtml     # Add new movie form
│       ├── EditMovie.cshtml       # Edit movie form
│       ├── Reviews.cshtml         # Review management table
│       └── Users.cshtml           # User management table
│
├── wwwroot/
│   ├── css/site.css               # Full cinematic dark-mode CSS (1100+ lines)
│   ├── js/site.js                 # Theme toggle, dropdown, animations
│   └── images/no-poster.svg       # Fallback poster image
│
├── Program.cs                     # App entry point, DI, middleware, seeding
├── appsettings.json
└── CriticsDB.csproj
```

---

## ✨ Features

### 👤 Authentication
- Register / Login / Logout with ASP.NET Identity
- Remember me, password visibility toggle
- Role-based: **Admin** and **User**

### 🎬 Movies
- Browse all movies with search, genre filter, sort (latest / top rated / most reviewed / oldest)
- Pagination (12 per page)
- Movie details page with synopsis, poster, trailer link

### ⭐ Rating System
- Interactive 1–10 star rater (AJAX, no page reload)
- One rating per user per movie (update allowed)
- Live average recalculation
- Rating distribution bar chart

### 📝 Reviews
- Write, edit, and delete your own reviews
- 1–10 score selector on review form
- Character counter (2000 max)
- Like / Dislike reviews (AJAX)
- Admin can delete any review

### 🛠️ Admin Panel
- Dashboard with total stats
- Full CRUD for movies (with poster preview)
- Delete any review
- User management: view, toggle Admin/User role, delete users

### 🎨 UI/UX
- **Cinematic dark theme** by default (Bebas Neue + DM Sans)
- **Light mode** toggle (persisted in localStorage)
- Responsive (mobile, tablet, desktop)
- Scroll animations, hover effects, toast notifications
- Empty states, loading feedback

---

## 🔧 Development Notes

### Adding a Migration
```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Regenerating Migrations from Scratch
```bash
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Switching to Azure SQL / Production SQL
Update `appsettings.json` with your connection string. No code changes needed.

---

## 📦 NuGet Packages Used

| Package | Version |
|---------|---------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.0 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.0 |
| Microsoft.AspNetCore.Identity.UI | 8.0.0 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.0 |

---

## 📸 Pages at a Glance

| Page | URL |
|------|-----|
| Home | `/` |
| All Movies | `/Movies` |
| Movie Details | `/Movies/Details/{id}` |
| Login | `/Account/Login` |
| Register | `/Account/Register` |
| Edit Profile | `/Account/EditProfile` |
| User Profile | `/Profile/Details/{id}` |
| Admin Dashboard | `/Admin` |
| Admin Movies | `/Admin/Movies` |
| Admin Reviews | `/Admin/Reviews` |
| Admin Users | `/Admin/Users` |
