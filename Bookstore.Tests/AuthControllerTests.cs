using Xunit; 
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using BookStore.API.Controllers;
using BookStore.API.Data;
using BookStore.API.RequestModels;
using BookStore.API.Services;

namespace Bookstore.Tests
{
    public class AuthControllerTests
    {
        // Helper method to generate a TokenService with mock configuration
        private static TokenService CreateTokenService()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForTesting123!");
            mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");

            return new TokenService(mockConfig.Object);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase(databaseName: "UnauthorizedTestDb")
                .Options;
            var context = new BookStoreContext(options);

            var tokenService = CreateTokenService();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.Login("wronguser", "wrongpass"))
                           .Returns((string?)null);

            var controller = new AuthController(context, tokenService, mockAuthService.Object);

            var loginRequest = new LoginRequest
            {
                Username = "wronguser",
                Password = "wrongpass"
            };

            var result = controller.Login(loginRequest);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void Login_ReturnsToken_WhenCredentialsAreValid()
        {
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase(databaseName: "ValidLoginTestDb")
                .Options;
            var context = new BookStoreContext(options);

            var tokenService = CreateTokenService();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.Login("josey", "secret"))
                           .Returns("mocked-jwt-token");

            var controller = new AuthController(context, tokenService, mockAuthService.Object);

            var loginRequest = new LoginRequest
            {
                Username = "josey",
                Password = "secret"
            };

            var result = controller.Login(loginRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Dictionary<string, string>>(okResult.Value);
            Assert.Equal("mocked-jwt-token", response["token"]);
        }

        [Fact]
        public void Register_ReturnsOk_WhenNewUser()
        {
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase("RegisterSuccessDb")
                .Options;
            var context = new BookStoreContext(options);

            var tokenService = CreateTokenService();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.Register("newuser", "newpass"))
                        .Returns(true);

            var controller = new AuthController(context, tokenService, mockAuthService.Object);

            var request = new RegisterRequest
            {
                Username = "newuser",
                Password = "newpass"
            };

            var result = controller.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Registration successful", okResult.Value);
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenUserExists()
        {
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase("RegisterFailDb")
                .Options;
            var context = new BookStoreContext(options);

            var tokenService = CreateTokenService();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.Register("existing", "pass"))
                        .Returns(false);

            var controller = new AuthController(context, tokenService, mockAuthService.Object);

            var request = new RegisterRequest
            {
                Username = "existing",
                Password = "pass"
            };

            var result = controller.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists.", badRequest.Value);
        }
    }
}
