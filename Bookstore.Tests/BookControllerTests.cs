using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookStore.API.Controllers;
using BookStore.API.Models;
using BookStore.API.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bookstore.Tests
{
    public class BookControllerTests
    {
        // Helper method to simulate an authenticated user by injecting claims into the controller's context
        private BookController SetupControllerWithUser(string username, Mock<IBookService> mockService)
        {
            var controller = new BookController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task AddBook_ReturnsCreatedAtAction_WithAddedBook()
        {
            // Arrange
            var mockService = new Mock<IBookService>();
            var sampleBook = new Book { Id = 1, Name = "Sample Book", CreatedBy = "josey" };

            // Setup mock behavior to return a sample book when AddBook is called
            mockService.Setup(s => s.AddBook(It.IsAny<Book>())).ReturnsAsync(sampleBook);

            // Simulate logged-in user "josey"
            var controller = SetupControllerWithUser("josey", mockService);
            var bookToAdd = new Book { Name = "Sample Book" };

            // Act
            var result = await controller.AddBook(bookToAdd);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result); // check result is 201 Created
            var returnedBook = Assert.IsType<Book>(createdResult.Value); // check returned object is a Book
            Assert.Equal("Sample Book", returnedBook.Name);
            Assert.Equal("josey", returnedBook.CreatedBy);
        }

        [Fact]
        public async Task UpdateBook_ReturnsOk_WhenBookIsUpdated()
        {
            // Arrange
            var mockService = new Mock<IBookService>();
            var username = "josey";

            // Original and updated book details
            var existingBook = new Book { Id = 1, Name = "Old Book", CreatedBy = username };
            var updatedBook = new Book { Name = "Updated Book", Category = "Fiction", Price = 10, Description = "Updated" };

            // Setup mocks to return expected values
            mockService.Setup(s => s.GetBookById(1)).ReturnsAsync(existingBook);
            mockService.Setup(s => s.UpdateBook(1, It.IsAny<Book>(), username)).ReturnsAsync(updatedBook);

            var controller = SetupControllerWithUser(username, mockService);

            // Act
            var result = await controller.UpdateBook(1, updatedBook);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Expecting 200 OK
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal("Updated Book", returnedBook.Name);
        }

        [Fact]
        public async Task DeleteBook_ReturnsOk_WhenBookIsDeleted()
        {
            // Arrange
            var mockService = new Mock<IBookService>();
            var username = "josey";

            var book = new Book { Id = 1, Name = "To Delete", CreatedBy = username };

            // Simulate successful deletion
            mockService.Setup(s => s.GetBookById(1)).ReturnsAsync(book);
            mockService.Setup(s => s.DeleteBook(1, username)).ReturnsAsync(true);

            var controller = SetupControllerWithUser(username, mockService);

            // Act
            var result = await controller.DeleteBook(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Expect 200 OK
            Assert.Equal("Book deleted", okResult.Value);
        }

        [Fact]
        public async Task UpdateBook_ReturnsForbid_WhenUserDoesNotOwnBook()
        {
            // Arrange
            var mockService = new Mock<IBookService>();

            // Book created by someone else
            var book = new Book { Id = 1, Name = "Not Yours", CreatedBy = "someone_else" };
            mockService.Setup(s => s.GetBookById(1)).ReturnsAsync(book);

            // Simulate logged-in user who is not the creator
            var controller = SetupControllerWithUser("josey", mockService);
            var update = new Book { Name = "Try to Update" };

            // Act
            var result = await controller.UpdateBook(1, update);

            // Assert
            Assert.IsType<ForbidResult>(result); // Expecting 403 Forbid
        }
    }
}
