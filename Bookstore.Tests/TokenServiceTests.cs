using Xunit;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Bookstore.Tests
{
    public class TokenServiceTests
    {
        [Fact]
        public void GenerateToken_ThrowsException_WhenUsernameIsNull()
        {
            // Arrange: TokenService with valid config
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "super secret jwt key that is definitely over 32 bytes!" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:DurationInMinutes", "60" }
            }).Build();

            var service = new TokenService(config);

            var invalidUser = new User
            {
                Username = null! // This will trigger an ArgumentException
            };

            // Act & Assert: Should throw ArgumentException
            var ex = Assert.Throws<ArgumentException>(() => service.GenerateToken(invalidUser));
            Assert.Equal("Username cannot be null or empty (Parameter 'Username')", ex.Message);
        }

        [Fact]
        public void GenerateToken_ThrowsException_WhenJwtKeyIsMissing()
        {
            // Arrange: Deliberately omit Jwt:Key from config
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:DurationInMinutes", "60" }
            }).Build();

            var service = new TokenService(config);

            var user = new User
            {
                Username = "josey"
            };

            // Act & Assert: Should throw InvalidOperationException due to missing key
            var ex = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(user));
            Assert.Equal("Jwt:Key is missing in configuration", ex.Message);
        }
    }
}
