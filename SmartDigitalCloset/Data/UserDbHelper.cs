using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartDigitalCloset.Data
{
    public class UserDbHelper
    {
        private readonly string connectionString;

        public UserDbHelper(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public User GetUserByEmail(string email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT * FROM Users WHERE Email = @Email", 
                    connection);
                command.Parameters.AddWithValue("@Email", email);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            ProfileImage = reader.IsDBNull(reader.GetOrdinal("ProfileImage")) ? null : reader.GetString(reader.GetOrdinal("ProfileImage")),
                            Preferences = reader.IsDBNull(reader.GetOrdinal("Preferences")) ? null : reader.GetString(reader.GetOrdinal("Preferences")),
                            FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                            Birthdate = reader.IsDBNull(reader.GetOrdinal("Birthdate")) ? null : reader.GetDateTime(reader.GetOrdinal("Birthdate"))
                        };
                    }
                    return null;
                }
            }
        }

        public User GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Email, CreatedAt FROM Users WHERE Id = @Id";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                CreatedAt = reader.GetDateTime(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void AddUser(User user)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"INSERT INTO Users (Email, Username, Password, PasswordHash, CreatedAt, 
                    FirstName, LastName, Birthdate) 
                    OUTPUT INSERTED.Id
                    VALUES (@Email, @Username, @Password, @PasswordHash, @CreatedAt, 
                    @FirstName, @LastName, @Birthdate)", 
                    connection);

                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                command.Parameters.AddWithValue("@FirstName", (object)user.FirstName ?? DBNull.Value);
                command.Parameters.AddWithValue("@LastName", (object)user.LastName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Birthdate", (object)user.Birthdate ?? DBNull.Value);

                // Get the inserted ID
                user.Id = (int)command.ExecuteScalar();
            }
        }

        public void UpdateUser(User user)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE Users SET Email = @Email, Username = @Username, Password = @Password, " +
                    "PasswordHash = @PasswordHash, ProfileImage = @ProfileImage, Preferences = @Preferences, " +
                    "FirstName = @FirstName, LastName = @LastName, Birthdate = @Birthdate " +
                    "WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@ProfileImage", (object)user.ProfileImage ?? DBNull.Value);
                command.Parameters.AddWithValue("@Preferences", (object)user.Preferences ?? DBNull.Value);
                command.Parameters.AddWithValue("@FirstName", (object)user.FirstName ?? DBNull.Value);
                command.Parameters.AddWithValue("@LastName", (object)user.LastName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Birthdate", (object)user.Birthdate ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteUser(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // First delete related records in ClosetItems and Outfits
                var deleteClosetItemsCmd = new SqlCommand("DELETE FROM ClosetItems WHERE UserId = @UserId", connection);
                deleteClosetItemsCmd.Parameters.AddWithValue("@UserId", id);
                deleteClosetItemsCmd.ExecuteNonQuery();

                var deleteOutfitsCmd = new SqlCommand("DELETE FROM Outfits WHERE UserId = @UserId", connection);
                deleteOutfitsCmd.Parameters.AddWithValue("@UserId", id);
                deleteOutfitsCmd.ExecuteNonQuery();

                // Then delete the user
                var deleteUserCmd = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection);
                deleteUserCmd.Parameters.AddWithValue("@Id", id);
                deleteUserCmd.ExecuteNonQuery();
            }
        }
    }
}