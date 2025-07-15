using Xunit;
using BookStore.API.RequestModels;

namespace Bookstore.Tests
{
    public class AuthModelTests
    {
        [Fact]
        public void RegisterRequest_CanBeInitialized()
        {
            // Arrange & Act: Create a new RegisterRequest and assign values
            var request = new RegisterRequest
            {
                Username = "josey",
                Password = "secure123"
            };

            // Assert: Check that the object stores the values correctly
            Assert.Equal("josey", request.Username);
            Assert.Equal("secure123", request.Password);
        }
    }
}
