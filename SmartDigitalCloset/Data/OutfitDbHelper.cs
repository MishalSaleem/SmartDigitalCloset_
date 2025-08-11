using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartDigitalCloset.Data
{
    public class OutfitDbHelper
    {
        private readonly string _connectionString;

        public OutfitDbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Outfit> GetOutfits(int userId)
        {
            var outfits = new List<Outfit>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT Id, UserId, Name, Tags, PlannedDate, ItemIds, CreatedAt FROM Outfits WHERE UserId = @UserId ORDER BY PlannedDate", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            outfits.Add(new Outfit
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Tags = reader.GetString(3),
                                PlannedDate = reader.GetDateTime(4),
                                ItemIds = reader.GetString(5),
                                CreatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            return outfits;
        }        public void AddOutfit(Outfit outfit)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    @"INSERT INTO Outfits (UserId, Name, Tags, PlannedDate, ItemIds, CreatedAt) 
                      VALUES (@UserId, @Name, @Tags, @PlannedDate, @ItemIds, @CreatedAt)", connection))
                {
                    command.Parameters.AddWithValue("@UserId", outfit.UserId);
                    command.Parameters.AddWithValue("@Name", outfit.Name);
                    command.Parameters.AddWithValue("@Tags", outfit.Tags);
                    command.Parameters.AddWithValue("@PlannedDate", outfit.PlannedDate);
                    command.Parameters.AddWithValue("@ItemIds", outfit.ItemIds);
                    command.Parameters.AddWithValue("@CreatedAt", outfit.CreatedAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateOutfit(Outfit outfit)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    @"UPDATE Outfits 
                      SET Name = @Name, Tags = @Tags, PlannedDate = @PlannedDate, ItemIds = @ItemIds 
                      WHERE Id = @Id AND UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@Id", outfit.Id);
                    command.Parameters.AddWithValue("@UserId", outfit.UserId);
                    command.Parameters.AddWithValue("@Name", outfit.Name);
                    command.Parameters.AddWithValue("@Tags", outfit.Tags);
                    command.Parameters.AddWithValue("@PlannedDate", outfit.PlannedDate);
                    command.Parameters.AddWithValue("@ItemIds", outfit.ItemIds);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOutfit(int id, int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "DELETE FROM Outfits WHERE Id = @Id AND UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
} 