using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartDigitalCloset.Data
{
    public class FavoriteItem
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = "";
        public string ItemType { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string ItemImageUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class FavoritesService
    {
        private readonly string _connectionString;        public FavoritesService(string connectionString)
        {
            _connectionString = connectionString;
            
            try
            {
                // Initialize the database in the background to avoid blocking the app startup
                Task.Run(async () => 
                {
                    await EnsureTableExistsAsync();
                    // Removed the automatic test favorite creation to ensure new users start with zero favorites
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FavoritesService constructor: {ex.Message}");
            }
        }
        
        // Kept this method but it's no longer called automatically
        private async Task EnsureTestFavoriteExistsAsync()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                
                // Check if any favorites exist
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Favorites", conn);
                int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                
                if (count == 0)
                {
                    // Add a test favorite if needed - now this is only for debugging
                    var cmd = new SqlCommand(@"
                        INSERT INTO Favorites (UserEmail, ItemType, ItemName, ItemImageUrl)
                        VALUES ('i@gmail.com', 'Fitness', 'Sample Exercise', 'https://via.placeholder.com/150')", conn);
                    await cmd.ExecuteNonQueryAsync();
                    
                    Console.WriteLine("Added test favorite record");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding test favorite: {ex.Message}");
            }
        }          private async Task EnsureTableExistsAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            try
            {
                await conn.OpenAsync();
                
                // First, ensure database exists
                var checkDbCmd = new SqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ClosetAppDB2')
                    BEGIN
                        CREATE DATABASE ClosetAppDB2;
                    END", conn);
                await checkDbCmd.ExecuteNonQueryAsync();
                
                // Now use the database
                var useDbCmd = new SqlCommand("USE ClosetAppDB2;", conn);
                await useDbCmd.ExecuteNonQueryAsync();
                
                // Create the favorites table if it doesn't exist
                var createTableCmd = new SqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Favorites')
                    BEGIN
                        CREATE TABLE Favorites (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserEmail NVARCHAR(100) NOT NULL,
                            ItemType NVARCHAR(50) NOT NULL,
                            ItemName NVARCHAR(100) NOT NULL,
                            ItemImageUrl NVARCHAR(MAX) NOT NULL,
                            CreatedAt DATETIME DEFAULT GETDATE()
                        );
                    END", conn);
                await createTableCmd.ExecuteNonQueryAsync();
                
                // Add CreatedAt column if it doesn't exist
                var addCreatedAtCmd = new SqlCommand(@"
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE name = 'CreatedAt' AND object_id = OBJECT_ID('Favorites')
                    )
                    BEGIN
                        ALTER TABLE Favorites ADD CreatedAt DATETIME DEFAULT GETDATE();
                        UPDATE Favorites SET CreatedAt = GETDATE();
                    END", conn);
                await addCreatedAtCmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring database/table exists: {ex.Message}");
                // Continue execution - we'll use memory storage as fallback
            }
        }// In-memory backup for favorites if database is unavailable
        private static Dictionary<string, List<FavoriteItem>> _memoryFavorites = new Dictionary<string, List<FavoriteItem>>(StringComparer.OrdinalIgnoreCase);

        public async Task AddFavoriteAsync(FavoriteItem item)
        {
            try
            {
                // Ensure table exists first
                await EnsureTableExistsAsync();
                
                // Normalize the email to lowercase to avoid case sensitivity issues
                item.UserEmail = item.UserEmail.ToLowerInvariant();
                
                // First check if the item already exists (to avoid duplicates)
                bool alreadyExists = false;
                
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Favorites WHERE LOWER(UserEmail) = LOWER(@UserEmail) AND ItemName = @ItemName", conn);
                    checkCmd.Parameters.AddWithValue("@UserEmail", item.UserEmail);
                    checkCmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                    alreadyExists = (count > 0);
                }
                
                if (alreadyExists)
                {
                    Console.WriteLine($"Item already exists in favorites: {item.ItemName}");
                    return;
                }
                  using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(@"
                        INSERT INTO Favorites (UserEmail, ItemType, ItemName, ItemImageUrl, CreatedAt) 
                        VALUES (@UserEmail, @ItemType, @ItemName, @ItemImageUrl, @CreatedAt)", conn);
                    cmd.Parameters.AddWithValue("@UserEmail", item.UserEmail);
                    cmd.Parameters.AddWithValue("@ItemType", item.ItemType);
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    cmd.Parameters.AddWithValue("@ItemImageUrl", item.ItemImageUrl);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    await cmd.ExecuteNonQueryAsync();
                    
                    Console.WriteLine($"Added to database: {item.ItemName} for {item.UserEmail}");
                }
                
                // Also store in memory as backup
                if (!_memoryFavorites.ContainsKey(item.UserEmail))
                {
                    _memoryFavorites[item.UserEmail] = new List<FavoriteItem>();
                }
                _memoryFavorites[item.UserEmail].Add(item);
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine($"Database error while adding favorite: {ex.Message}");
                
                // Store in memory as fallback
                if (!_memoryFavorites.ContainsKey(item.UserEmail))
                {
                    _memoryFavorites[item.UserEmail] = new List<FavoriteItem>();
                }
                _memoryFavorites[item.UserEmail].Add(item);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while adding favorite: {ex.Message}");
                
                // Store in memory as fallback
                if (!_memoryFavorites.ContainsKey(item.UserEmail))
                {
                    _memoryFavorites[item.UserEmail] = new List<FavoriteItem>();
                }
                _memoryFavorites[item.UserEmail].Add(item);
            }
        }        public async Task<List<FavoriteItem>> GetFavoritesAsync(string userEmail)
        {
            // Normalize email to lowercase
            userEmail = userEmail?.ToLowerInvariant() ?? string.Empty;
            
            var result = new List<FavoriteItem>();
            bool databaseSuccess = false;
            
            try
            {
                // Ensure the table exists first
                await EnsureTableExistsAsync();
                
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                  // Use a case-insensitive comparison for email and order by most recent
                var cmd = new SqlCommand(@"
                    SELECT * FROM Favorites 
                    WHERE LOWER(UserEmail) = LOWER(@UserEmail) 
                    ORDER BY CreatedAt DESC", conn);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new FavoriteItem
                    {
                        Id = (int)reader["Id"],
                        UserEmail = (string)reader["UserEmail"],
                        ItemType = (string)reader["ItemType"],
                        ItemName = (string)reader["ItemName"],
                        ItemImageUrl = (string)reader["ItemImageUrl"],
                        CreatedAt = reader["CreatedAt"] == DBNull.Value 
                            ? DateTime.Now 
                            : Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
                databaseSuccess = true;
                
                Console.WriteLine($"Successfully retrieved {result.Count} favorites for {userEmail}");
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine($"Database error while retrieving favorites: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while retrieving favorites: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            
            // If database failed, use in-memory backup
            if (!databaseSuccess && _memoryFavorites.ContainsKey(userEmail))
            {
                return _memoryFavorites[userEmail];
            }
            
            return result;
        }public async Task RemoveFavoriteAsync(string userEmail, string itemName)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "DELETE FROM Favorites WHERE UserEmail = @UserEmail AND ItemName = @ItemName", conn);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                cmd.Parameters.AddWithValue("@ItemName", itemName);
                await cmd.ExecuteNonQueryAsync();
                
                // Also remove from memory backup
                if (_memoryFavorites.ContainsKey(userEmail))
                {
                    _memoryFavorites[userEmail].RemoveAll(f => f.ItemName == itemName);
                }
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine($"Database error while removing favorite: {ex.Message}");
                
                // Remove from memory backup instead
                if (_memoryFavorites.ContainsKey(userEmail))
                {
                    _memoryFavorites[userEmail].RemoveAll(f => f.ItemName == itemName);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while removing favorite: {ex.Message}");
                
                // Remove from memory backup instead
                if (_memoryFavorites.ContainsKey(userEmail))
                {
                    _memoryFavorites[userEmail].RemoveAll(f => f.ItemName == itemName);
                }
            }
        }
        
        /// <summary>
        /// Gets the count of favorites by category for a user
        /// </summary>
        public async Task<Dictionary<string, int>> GetFavoritesCategoryCountAsync(string userEmail)
        {
            // Normalize email to lowercase
            userEmail = userEmail?.ToLowerInvariant() ?? string.Empty;
            
            var result = new Dictionary<string, int>();
            bool databaseSuccess = false;
            
            try
            {
                // Ensure the table exists first
                await EnsureTableExistsAsync();
                
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                
                // Use a case-insensitive comparison for email
                var cmd = new SqlCommand(@"
                    SELECT ItemType, COUNT(*) as ItemCount 
                    FROM Favorites 
                    WHERE LOWER(UserEmail) = LOWER(@UserEmail)
                    GROUP BY ItemType
                    ORDER BY ItemCount DESC", conn);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string category = reader["ItemType"].ToString() ?? "Unknown";
                    int count = Convert.ToInt32(reader["ItemCount"]);
                    result[category] = count;
                }
                
                databaseSuccess = true;
                Console.WriteLine($"Successfully retrieved category counts for {userEmail}");
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine($"Database error while retrieving category counts: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while retrieving category counts: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            
            // If database failed, use in-memory backup
            if (!databaseSuccess && _memoryFavorites.ContainsKey(userEmail))
            {
                return _memoryFavorites[userEmail]
                    .GroupBy(f => f.ItemType)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            
            return result;
        }

        /// <summary>
        /// Gets favorites for a specific category
        /// </summary>
        public async Task<List<FavoriteItem>> GetFavoritesByCategoryAsync(string userEmail, string category)
        {
            // Normalize email to lowercase
            userEmail = userEmail?.ToLowerInvariant() ?? string.Empty;
            category = category ?? ""; // Default to empty string if null
            
            var result = new List<FavoriteItem>();
            bool databaseSuccess = false;
            
            try
            {
                // Ensure the table exists first
                await EnsureTableExistsAsync();
                
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                
        // Use a case-insensitive comparison for both email and category
        var cmd = new SqlCommand(@"
            SELECT * FROM Favorites 
            WHERE LOWER(UserEmail) = LOWER(@UserEmail) 
            AND LOWER(ItemType) = LOWER(@Category)
            ORDER BY CreatedAt DESC", conn);
        cmd.Parameters.AddWithValue("@UserEmail", userEmail);
        cmd.Parameters.AddWithValue("@Category", category);
        
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new FavoriteItem
            {
                Id = (int)reader["Id"],
                UserEmail = (string)reader["UserEmail"],
                ItemType = (string)reader["ItemType"],
                ItemName = (string)reader["ItemName"],
                ItemImageUrl = (string)reader["ItemImageUrl"],
                CreatedAt = reader["CreatedAt"] == DBNull.Value 
                    ? DateTime.Now 
                    : Convert.ToDateTime(reader["CreatedAt"])
            });
        }
                
                databaseSuccess = true;
                Console.WriteLine($"Successfully retrieved {result.Count} favorites in category '{category}' for {userEmail}");
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine($"Database error while retrieving favorites by category: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while retrieving favorites by category: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            
            // If database failed, use in-memory backup
            if (!databaseSuccess && _memoryFavorites.ContainsKey(userEmail))
            {
                return _memoryFavorites[userEmail]
                    .Where(f => string.Equals(f.ItemType, category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            
            return result;
        }

        /// <summary>
        /// Gets favorites added on a specific date
        /// </summary>
        public async Task<List<FavoriteItem>> GetFavoritesByDateAsync(string userEmail, DateTime date)
        {
            // Normalize email to lowercase
            userEmail = userEmail?.ToLowerInvariant() ?? string.Empty;
            
            var result = new List<FavoriteItem>();
            bool databaseSuccess = false;
            
            try
            {
                // Ensure the table exists first
                await EnsureTableExistsAsync();
                
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                
                // Get favorites added on the specific date
                var cmd = new SqlCommand(@"
                    SELECT * FROM Favorites 
                    WHERE LOWER(UserEmail) = LOWER(@UserEmail) 
                    AND CONVERT(date, CreatedAt) = CONVERT(date, @Date)
                    ORDER BY CreatedAt DESC", conn);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new FavoriteItem
                    {
                        Id = (int)reader["Id"],
                        UserEmail = (string)reader["UserEmail"],
                        ItemType = (string)reader["ItemType"],
                        ItemName = (string)reader["ItemName"],
                        ItemImageUrl = (string)reader["ItemImageUrl"],
                        CreatedAt = reader["CreatedAt"] == DBNull.Value 
                            ? DateTime.Now 
                            : Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
                
                databaseSuccess = true;
                Console.WriteLine($"Successfully retrieved {result.Count} favorites added on {date.ToShortDateString()} for {userEmail}");
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine($"Database error while retrieving favorites by date: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while retrieving favorites by date: {ex.Message}");
                // Fall back to memory storage
                databaseSuccess = false;
            }
            
            // If database failed, use in-memory backup
            if (!databaseSuccess && _memoryFavorites.ContainsKey(userEmail))
            {
                return _memoryFavorites[userEmail]
                    .Where(f => f.CreatedAt.Date == date.Date)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToList();
            }
            
            return result;
        }

        /// <summary>
        /// Checks if the database connection is working
        /// </summary>
        public async Task<(bool Success, string Message)> TestDatabaseConnectionAsync()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                
                // Try a simple query
                var cmd = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = 'ClosetAppDB2'", conn);
                int dbExists = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                
                if (dbExists > 0)
                {
                    // Now check if the table exists
                    cmd = new SqlCommand("USE ClosetAppDB2; SELECT COUNT(*) FROM sys.tables WHERE name = 'Favorites'", conn);
                    int tableExists = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    
                    if (tableExists > 0)
                    {
                        // Check if there are any favorites
                        cmd = new SqlCommand("USE ClosetAppDB2; SELECT COUNT(*) FROM Favorites", conn);
                        int favCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        
                        return (true, $"Database connection successful. Found {favCount} total favorites.");
                    }
                    else
                    {
                        return (true, "Database exists but Favorites table doesn't exist yet. It will be created automatically when needed.");
                    }
                }
                else
                {
                    return (true, "Connected to SQL Server but database 'ClosetAppDB2' doesn't exist yet. It will be created automatically when needed.");
                }
            }
            catch (SqlException ex)
            {
                return (false, $"Database connection failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
