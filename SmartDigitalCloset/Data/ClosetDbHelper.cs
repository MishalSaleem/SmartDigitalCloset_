using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartDigitalCloset.Data
{
    public class ClosetItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }

    public class ClosetDbHelper
    {
        private readonly string _connectionString;
        public ClosetDbHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public void AddItem(ClosetItem item)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("INSERT INTO ClosetItems (UserId, Type, Season, Color, ImagePath) VALUES (@UserId, @Type, @Season, @Color, @ImagePath)", con);
            cmd.Parameters.AddWithValue("@UserId", item.UserId);
            cmd.Parameters.AddWithValue("@Type", item.Type);
            cmd.Parameters.AddWithValue("@Season", item.Season);
            cmd.Parameters.AddWithValue("@Color", item.Color);
            cmd.Parameters.AddWithValue("@ImagePath", item.ImagePath);
            cmd.ExecuteNonQuery();
        }

        public List<ClosetItem> GetItems(int userId)
        {
            var list = new List<ClosetItem>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("SELECT * FROM ClosetItems WHERE UserId = @UserId", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ClosetItem
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    Type = reader["Type"].ToString()!,
                    Season = reader["Season"].ToString()!,
                    Color = reader["Color"].ToString()!,
                    ImagePath = reader["ImagePath"].ToString()!
                });
            }
            return list;
        }

        public void UpdateItem(ClosetItem item)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("UPDATE ClosetItems SET Type=@Type, Season=@Season, Color=@Color, ImagePath=@ImagePath WHERE Id=@Id AND UserId=@UserId", con);
            cmd.Parameters.AddWithValue("@Id", item.Id);
            cmd.Parameters.AddWithValue("@UserId", item.UserId);
            cmd.Parameters.AddWithValue("@Type", item.Type);
            cmd.Parameters.AddWithValue("@Season", item.Season);
            cmd.Parameters.AddWithValue("@Color", item.Color);
            cmd.Parameters.AddWithValue("@ImagePath", item.ImagePath);
            cmd.ExecuteNonQuery();
        }

        public void DeleteItem(int id, int userId)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("DELETE FROM ClosetItems WHERE Id=@Id AND UserId=@UserId", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.ExecuteNonQuery();
        }
    }
}
