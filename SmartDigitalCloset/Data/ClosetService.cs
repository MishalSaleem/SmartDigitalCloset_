using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace SmartDigitalCloset.Data
{    public class ClosetService
    {
        private readonly string _jsonPath;
        private readonly object _lock = new();
        private readonly ClosetDbHelper? _dbHelper;
        private readonly int _defaultUserId = 1; // Default user ID for demo purposes
        private readonly bool _useDatabase;
        
        public ClosetService(IWebHostEnvironment env, IConfiguration? config = null)
        {
            _jsonPath = Path.Combine(env.WebRootPath, "data", "closet.json");
            _useDatabase = config != null;
            
            if (_useDatabase)
            {
                try
                {
                    _dbHelper = new ClosetDbHelper(config!);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to initialize database: {ex.Message}");
                    _useDatabase = false;
                }
            }
        }        public async Task<List<ClosetItem>> LoadItemsAsync()
        {
            try
            {
                // Always use database if enabled
                if (_useDatabase && _dbHelper != null)
                {
                    try
                    {
                        var dbItems = _dbHelper.GetItems(_defaultUserId);
                        return dbItems ?? new List<ClosetItem>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading items from database: {ex.Message}");
                        // Fall back to JSON file
                    }
                }
                
                // Load from JSON file
                if (!File.Exists(_jsonPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
                    await File.WriteAllTextAsync(_jsonPath, "[]");
                }
                using var stream = File.OpenRead(_jsonPath);
                var items = await JsonSerializer.DeserializeAsync<List<ClosetItem>>(stream) ?? new List<ClosetItem>();
                
                // Sync items to database if needed and database is enabled
                if (_useDatabase && _dbHelper != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        item.UserId = _defaultUserId;
                        try { _dbHelper.AddItem(item); } catch { /* Ignore if item already exists */ }
                    }
                }
                
                return items;
            }            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadItemsAsync: {ex.Message}");
                return new List<ClosetItem>();
            }
        }        public Task<bool> SaveItemAsync(ClosetItem item)
        {
            try
            {
                // Ensure the item has a user ID
                item.UserId = _defaultUserId;
                
                // Save to JSON file
                lock (_lock)
                {
                    var items = new List<ClosetItem>();
                    if (File.Exists(_jsonPath))
                    {
                        using var stream = File.OpenRead(_jsonPath);
                        items = JsonSerializer.Deserialize<List<ClosetItem>>(stream) ?? new List<ClosetItem>();
                    }
                    item.Id = items.Count > 0 ? items[^1].Id + 1 : 1;
                    items.Add(item);
                    var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(_jsonPath, json);
                }
                
                // Save to database if enabled
                if (_useDatabase && _dbHelper != null)
                {
                    try
                    {
                        _dbHelper.AddItem(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding item to database: {ex.Message}");
                        // Continue execution - we at least saved to JSON
                    }
                }
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveItemAsync: {ex.Message}");
                return Task.FromResult(false);
            }
        }        public Task<bool> DeleteItemAsync(int itemId)
        {
            try
            {
                // Delete from JSON file
                lock (_lock)
                {
                    var items = new List<ClosetItem>();
                    if (File.Exists(_jsonPath))
                    {
                        using var stream = File.OpenRead(_jsonPath);
                        items = JsonSerializer.Deserialize<List<ClosetItem>>(stream) ?? new List<ClosetItem>();
                    }
                    
                    var itemToRemove = items.FirstOrDefault(i => i.Id == itemId);
                    if (itemToRemove != null)
                    {
                        items.Remove(itemToRemove);
                        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(_jsonPath, json);
                    }
                }
                
                // Delete from database if enabled
                if (_useDatabase && _dbHelper != null)
                {
                    try
                    {
                        _dbHelper.DeleteItem(itemId, _defaultUserId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting item from database: {ex.Message}");
                        // Continue execution - we at least deleted from JSON
                    }
                }
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteItemAsync: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
