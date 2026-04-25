using Microsoft.Data.Sqlite;

namespace CriticsDB.Data
{
    /// <summary>
    /// Creates all tables directly via raw SQL — no EF migrations needed.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task InitializeAsync(string connectionString)
        {
            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            var sql = @"
PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS AspNetRoles (
    Id TEXT NOT NULL PRIMARY KEY,
    Name TEXT,
    NormalizedName TEXT,
    ConcurrencyStamp TEXT
);

CREATE TABLE IF NOT EXISTS AspNetUsers (
    Id TEXT NOT NULL PRIMARY KEY,
    DisplayName TEXT NOT NULL DEFAULT '',
    Bio TEXT,
    AvatarUrl TEXT,
    JoinedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UserName TEXT,
    NormalizedUserName TEXT,
    Email TEXT,
    NormalizedEmail TEXT,
    EmailConfirmed INTEGER NOT NULL DEFAULT 0,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    ConcurrencyStamp TEXT,
    PhoneNumber TEXT,
    PhoneNumberConfirmed INTEGER NOT NULL DEFAULT 0,
    TwoFactorEnabled INTEGER NOT NULL DEFAULT 0,
    LockoutEnd TEXT,
    LockoutEnabled INTEGER NOT NULL DEFAULT 0,
    AccessFailedCount INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS AspNetUserRoles (
    UserId TEXT NOT NULL,
    RoleId TEXT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetUserClaims (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetUserLogins (
    LoginProvider TEXT NOT NULL,
    ProviderKey TEXT NOT NULL,
    ProviderDisplayName TEXT,
    UserId TEXT NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetUserTokens (
    UserId TEXT NOT NULL,
    LoginProvider TEXT NOT NULL,
    Name TEXT NOT NULL,
    Value TEXT,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    RoleId TEXT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Movies (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL DEFAULT '',
    Description TEXT NOT NULL DEFAULT '',
    Genre TEXT NOT NULL DEFAULT '',
    ReleaseDate TEXT NOT NULL,
    PosterUrl TEXT,
    TrailerUrl TEXT,
    Director TEXT,
    Type TEXT NOT NULL DEFAULT 'Movie',
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Reviews (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    MovieId INTEGER NOT NULL,
    Comment TEXT NOT NULL DEFAULT '',
    Rating INTEGER NOT NULL DEFAULT 5,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Ratings (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    MovieId INTEGER NOT NULL,
    Value INTEGER NOT NULL DEFAULT 5,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ReviewLikes (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    ReviewId INTEGER NOT NULL,
    IsLike INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (ReviewId) REFERENCES Reviews(Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_AspNetRoles_NormalizedName ON AspNetRoles(NormalizedName);
CREATE UNIQUE INDEX IF NOT EXISTS IX_AspNetUsers_NormalizedUserName ON AspNetUsers(NormalizedUserName);
CREATE INDEX IF NOT EXISTS IX_AspNetUsers_NormalizedEmail ON AspNetUsers(NormalizedEmail);
CREATE INDEX IF NOT EXISTS IX_AspNetUserRoles_RoleId ON AspNetUserRoles(RoleId);
CREATE INDEX IF NOT EXISTS IX_AspNetUserClaims_UserId ON AspNetUserClaims(UserId);
CREATE INDEX IF NOT EXISTS IX_AspNetUserLogins_UserId ON AspNetUserLogins(UserId);
CREATE INDEX IF NOT EXISTS IX_Reviews_MovieId ON Reviews(MovieId);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Reviews_UserId_MovieId ON Reviews(UserId, MovieId);
CREATE INDEX IF NOT EXISTS IX_Ratings_MovieId ON Ratings(MovieId);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Ratings_UserId_MovieId ON Ratings(UserId, MovieId);
CREATE INDEX IF NOT EXISTS IX_ReviewLikes_ReviewId ON ReviewLikes(ReviewId);
CREATE UNIQUE INDEX IF NOT EXISTS IX_ReviewLikes_UserId_ReviewId ON ReviewLikes(UserId, ReviewId);

-- EF migrations history table so EF doesn't complain
CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
    MigrationId TEXT NOT NULL PRIMARY KEY,
    ProductVersion TEXT NOT NULL
);

INSERT OR IGNORE INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20240101000000_InitialCreate', '8.0.0');
";
            await using var cmd = new SqliteCommand(sql, connection);
            await cmd.ExecuteNonQueryAsync();

            // Migrate existing DBs: add Type column if missing
            await using var checkCmd = new SqliteCommand(
                "SELECT COUNT(*) FROM pragma_table_info('Movies') WHERE name='Type';", connection);
            var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
            if (count == 0)
            {
                await using var alterCmd = new SqliteCommand(
                    "ALTER TABLE Movies ADD COLUMN Type TEXT NOT NULL DEFAULT 'Movie';", connection);
                await alterCmd.ExecuteNonQueryAsync();
            }
        }
    }
}