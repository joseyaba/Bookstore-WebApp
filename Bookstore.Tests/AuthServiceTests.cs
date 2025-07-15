using Xunit;
using BookStore.API.Services;
using BookStore.API.Data;
using BookStore.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Bookstore.Tests
{
    public class AuthServiceTests
    {
        // Helper to create an AuthService with a pre-added user
        private AuthService CreateAuthServiceWithUser(string username, string password, out BookStoreContext context)
        {
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
                .Options;

            context = new BookStoreContext(options);

            // Create a user with a hashed password
            using var hmac = new HMACSHA256();
            var user = new User
            {
                Username = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };

            context.Users.Add(user);
            context.SaveChanges();

            // Set up mock configuration
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AppSettings:Token", "super secret key for jwt" }
            }).Build();

            var tokenService = new TokenService(configuration);
            return new AuthService(tokenService, context);
        }

        [Fact]
        public void Login_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange: Add a valid user manually to context
            var username = "josey";
            var password = "password123";

            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
                .Options;

            using var context = new BookStoreContext(options);

            // Hash the password with salt
            using var hmac = new HMACSHA256();
            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            var user = new User
            {
                Username = username,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash
            };

            context.Users.Add(user);
            context.SaveChanges();

            // Setup configuration
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "super secret jwt key that is definitely over 32 bytes!" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:DurationInMinutes", "60" }
            }).Build();

            var tokenService = new TokenService(configuration);
            var authService = new AuthService(tokenService, context);

            // Act: Attempt login with correct credentials
            var token = authService.Login(username, password);

            // Assert: Token should be returned
            Assert.NotNull(token);
            Assert.True(token.Length > 10);
        }

        [Fact]
        public void Login_ReturnsNull_WhenUserNotFound()
        {
            // Arrange: User doesn't exist in DB
            var username = "nonexistent";
            var password = "wrongpassword";
            var authService = CreateAuthServiceWithUser("someone", "realpassword", out _);

            // Act
            var token = authService.Login(username, password);

            // Assert: Login should fail
            Assert.Null(token);
        }

        [Fact]
        public void Login_ReturnsNull_WhenPasswordIsWrong()
        {
            // Arrange: Correct user but wrong password
            var username = "josey";
            var realPassword = "correctpassword";
            var wrongPassword = "incorrect";
            var authService = CreateAuthServiceWithUser(username, realPassword, out _);

            // Act
            var token = authService.Login(username, wrongPassword);

            // Assert: Login should fail
            Assert.Null(token);
        }
    }
}
