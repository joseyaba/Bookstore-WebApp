using BookStore.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace BookStore.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        // Injects app settings (e.g. Jwt:Key, Jwt:Issuer) from configuration
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generates a JWT token for a given user
        public string GenerateToken(User user)
        {
            // Ensure username is not null or empty
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username cannot be null or empty", nameof(user.Username));

            // Define claims for the token payload
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Load signing key from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing in configuration")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds);

            // Return serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Helper method to verify password hash with stored salt
        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
