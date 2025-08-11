using System.Security.Cryptography;
using System.Text;
using SmartDigitalCloset.Data;

namespace SmartDigitalCloset.Services
{
    public interface IAuthenticationService
    {
        bool ValidateUser(string email, string password);
        User RegisterUser(User user);
        User GetUserByEmail(string email);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserDbHelper _userDbHelper;

        public AuthenticationService(UserDbHelper userDbHelper)
        {
            _userDbHelper = userDbHelper;
        }

        public bool ValidateUser(string email, string password)
        {
            var user = _userDbHelper.GetUserByEmail(email);
            if (user == null) return false;

            var hashedPassword = HashPassword(password);
            return user.PasswordHash == hashedPassword;
        }

        public User RegisterUser(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = HashPassword(user.Password);
            _userDbHelper.AddUser(user);
            return user;
        }

        public User GetUserByEmail(string email)
        {
            return _userDbHelper.GetUserByEmail(email);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
} 