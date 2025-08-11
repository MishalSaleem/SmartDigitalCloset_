using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SmartDigitalCloset.Data
{
    public class OutfitService
    {
        private readonly string _jsonPath;
        private readonly object _lock = new();
        private readonly OutfitDbHelper? _dbHelper;
        private readonly int _defaultUserId = 1; // Default user ID for demo purposes
        private readonly bool _useDatabase;
        
        public OutfitService(IWebHostEnvironment env, IConfiguration? config = null)
        {
            _jsonPath = Path.Combine(env.WebRootPath, "data", "outfits.json");
            _useDatabase = config != null;
            
            if (_useDatabase)
            {
                try
                {
                    _dbHelper = new OutfitDbHelper(config!);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to initialize outfit database: {ex.Message}");
                    _useDatabase = false;
                }
            }
        }

        public async Task<List<Outfit>> LoadOutfitsAsync()
        {
            try
            {
                // Always use database if enabled
                if (_useDatabase && _dbHelper != null)
                {
                    try
                    {
                        var dbOutfits = _dbHelper.GetOutfits(_defaultUserId);
                        return dbOutfits ?? new List<Outfit>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading outfits from database: {ex.Message}");
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
                var outfits = await JsonSerializer.DeserializeAsync<List<Outfit>>(stream) ?? new List<Outfit>();
                
                // Sync outfits to database if needed and database is enabled
                if (_useDatabase && _dbHelper != null && outfits.Count > 0)
                {
                    foreach (var outfit in outfits)
                    {
                        outfit.UserId = _defaultUserId;
                        try { _dbHelper.AddOutfit(outfit); } catch { /* Ignore if outfit already exists */ }
                    }
                }
                
                return outfits;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadOutfitsAsync: {ex.Message}");
                return new List<Outfit>();
            }
        }        public Task<bool> SaveOutfitAsync(Outfit outfit)
        {
            try
            {
                // Ensure the outfit has a user ID
                outfit.UserId = _defaultUserId;
                
                // Track if this is a new outfit before we assign an ID
                bool isNewOutfit = outfit.Id <= 0;
                
                // Save to JSON file
                lock (_lock)
                {
                    var outfits = new List<Outfit>();
                    if (File.Exists(_jsonPath))
                    {
                        using var stream = File.OpenRead(_jsonPath);
                        outfits = JsonSerializer.Deserialize<List<Outfit>>(stream) ?? new List<Outfit>();
                    }
                    
                    // Set ID if not set
                    if (outfit.Id <= 0)
                    {
                        outfit.Id = outfits.Count > 0 ? outfits.Max(o => o.Id) + 1 : 1;
                    }
                    else
                    {
                        // If outfit already exists, update it
                        var existingIndex = outfits.FindIndex(o => o.Id == outfit.Id);
                        if (existingIndex >= 0)
                        {
                            outfits.RemoveAt(existingIndex);
                        }
                    }
                    
                    outfits.Add(outfit);
                    var json = JsonSerializer.Serialize(outfits, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(_jsonPath, json);
                }
                
                // Save to database if enabled
                if (_useDatabase && _dbHelper != null)
                {
                    try
                    {
                        // Use the original state to determine add vs update
                        if (isNewOutfit)
                        {
                            _dbHelper.AddOutfit(outfit);
                        }
                        else
                        {
                            _dbHelper.UpdateOutfit(outfit);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving outfit to database: {ex.Message}");
                        // Continue execution - we at least saved to JSON
                    }
                }
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveOutfitAsync: {ex.Message}");
                return Task.FromResult(false);
            }
        }
          public Task<bool> DeleteOutfitAsync(int outfitId)
        {
            try
            {
                // Delete from JSON file
                lock (_lock)
                {
                    var outfits = new List<Outfit>();
                    if (File.Exists(_jsonPath))
                    {
                        using var stream = File.OpenRead(_jsonPath);
                        outfits = JsonSerializer.Deserialize<List<Outfit>>(stream) ?? new List<Outfit>();
                    }
                    
                    var outfitToRemove = outfits.FirstOrDefault(o => o.Id == outfitId);
                    if (outfitToRemove != null)
                    {
                        outfits.Remove(outfitToRemove);
                        var json = JsonSerializer.Serialize(outfits, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(_jsonPath, json);
                    }
                }
                
                // Delete from database if enabled
                if (_useDatabase && _dbHelper != null)
                {
                    try
                    {
                        _dbHelper.DeleteOutfit(outfitId, _defaultUserId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting outfit from database: {ex.Message}");
                        // Continue execution - we at least deleted from JSON
                    }
                }
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteOutfitAsync: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
