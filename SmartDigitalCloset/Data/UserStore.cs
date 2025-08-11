using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartDigitalCloset.Data
{
    public class UserLegacy
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfileImageBase64 { get; set; } = string.Empty;
    }    public class UserStore
    {
        private readonly string FilePath;

        public UserStore()
        {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.txt");
        }

        public void AddUser(UserLegacy user)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);
            var line = $"{user.Email},{user.Password},{user.Birthdate:yyyy-MM-dd},{user.FirstName},{user.LastName},{user.ProfileImageBase64}";
            File.AppendAllLines(FilePath, new[] { line });
        }

        public UserLegacy? ValidateUser(string email, string password)
        {
            if (!File.Exists(FilePath)) return null;
            var lines = File.ReadAllLines(FilePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                var parts = line.Split(',');
                if (parts.Length < 6) continue;
                if (parts[0].Equals(email, StringComparison.OrdinalIgnoreCase) && parts[1] == password)
                {
                    DateTime.TryParse(parts[2], out var birthdate);
                    return new UserLegacy {
                        Email = parts[0],
                        Password = parts[1],
                        Birthdate = birthdate,
                        FirstName = parts[3],
                        LastName = parts[4],
                        ProfileImageBase64 = parts[5]
                    };
                }
            }
            return null;
        }        // Add method to convert UserLegacy to User
        public User ConvertToUser(UserLegacy legacy)
        {
            return new User
            {
                Email = legacy.Email,
                CreatedAt = DateTime.Now,
                ProfileImage = legacy.ProfileImageBase64,
                FirstName = legacy.FirstName,
                LastName = legacy.LastName,
                Birthdate = legacy.Birthdate,
                Preferences = $"FirstName:{legacy.FirstName},LastName:{legacy.LastName},Birthdate:{legacy.Birthdate:yyyy-MM-dd}"
            };
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProfileImage { get; set; }
        public string Preferences { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }

        public User()
        {
            Email = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            PasswordHash = string.Empty;
            ProfileImage = string.Empty;
            Preferences = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
        }
    }
}
