using BookStore.API.Models;
using BookStore.API.Data;
using BookStore.API.Services;
using System.Security.Cryptography;
using System.Text;

namespace BookStore.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly TokenService _tokenService;
        private readonly BookStoreContext _context;

        // Injects TokenService and database context
        public AuthService(TokenService tokenService, BookStoreContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        // Attempts login and returns JWT token if successful
        public string? Login(string username, string password)
        {
            // Look for user by username
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
                return null;

            // Recompute hash using stored salt
            using var hmac = new HMACSHA256(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Compare byte-by-byte
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return null;
            }

            // If password is valid, generate and return JWT
            return _tokenService.GenerateToken(user);
        }

        // Registers a new user
        public bool Register(string username, string password)
        {
            // Prevent duplicate usernames
            if (_context.Users.Any(u => u.Username == username))
                return false;

            // Generate salt and hash password
            using var hmac = new HMACSHA256();
            var user = new User
            {
                Username = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };

            // Save user to DB
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }
    }
}
