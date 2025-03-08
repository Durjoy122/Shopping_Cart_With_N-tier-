using DAL.Models;
using DAL.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool RegisterUser(User user)
        {
            if(_userRepository.GetByEmail(user.Email) != null)
            {
                return false;
            }
            user.PasswordHash = HashPassword(user.PasswordHash);
            _userRepository.Add(user);
            return true;
        }

        public User? ValidateUser(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null || user.PasswordHash != HashPassword(password))
            {
                return null;
            }
            return user;
        }
        private string HashPassword(string password)
        {
            using(var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}