using Xunit;
using BookStore.API.Models;
using BookStore.API.Services;
using BookStore.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Bookstore.Tests
{
    public class BookServiceTests
    {
        // Helper method to generate a new in-memory DB context for each test
        private BookStoreContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new BookStoreContext(options);
        }

        [Fact]
        public async Task AddBook_ShouldAddAndReturnBook()
        {
            // Arrange
            var context = GetInMemoryDbContext("AddBookDB");
            var service = new BookService(context);

            var newBook = new Book
            {
                Name = "Test Book",
                Category = "Fiction",
                Price = 50,
                Description = "Test Desc",
                CreatedBy = "josey"
            };

            // Act
            var result = await service.AddBook(newBook);

            // Assert
            Assert.NotNull(result); // Ensure a book was returned
            Assert.Equal("Test Book", result.Name); // Check name matches
            Assert.Equal(1, context.Books.Count()); // Ensure book was added to DB
        }

        [Fact]
        public async Task GetBookById_ShouldReturnBook_WhenExists()
        {
            // Arrange
            var context = GetInMemoryDbContext("GetBookDB");
            var service = new BookService(context);

            var book = new Book
            {
                Name = "Find Me",
                CreatedBy = "josey"
            };

            context.Books.Add(book);
            context.SaveChanges();

            // Act
            var result = await service.GetBookById(book.Id);

            // Assert
            Assert.NotNull(result); // Book should be found
            Assert.Equal("Find Me", result.Name);
        }

        [Fact]
        public async Task UpdateBook_ShouldUpdate_WhenUserOwnsBook()
        {
            // Arrange
            var context = GetInMemoryDbContext("UpdateBookDB");
            var service = new BookService(context);

            // Original book created by josey
            var original = new Book
            {
                Name = "Old Name",
                Category = "Drama",
                Price = 20,
                Description = "Old Desc",
                CreatedBy = "josey"
            };

            context.Books.Add(original);
            context.SaveChanges();

            // New values to update
            var updated = new Book
            {
                Name = "New Name",
                Category = "Action",
                Price = 25,
                Description = "New Desc"
            };

            // Act
            var result = await service.UpdateBook(original.Id, updated, "josey");

            // Assert
            Assert.NotNull(result); // Should return updated book
            Assert.Equal("New Name", result.Name);
            Assert.Equal("Action", result.Category);
        }

        [Fact]
        public async Task UpdateBook_ReturnsNull_WhenUserNotOwner()
        {
            // Arrange
            var context = GetInMemoryDbContext("UpdateFailDB");
            var service = new BookService(context);

            // Book created by 'owner'
            var book = new Book
            {
                Name = "Private",
                CreatedBy = "owner"
            };

            context.Books.Add(book);
            context.SaveChanges();

            // Act - try updating as a different user
            var result = await service.UpdateBook(book.Id, new Book { Name = "Hack" }, "intruder");

            // Assert
            Assert.Null(result); // Should return null (unauthorized)
        }

        [Fact]
        public async Task DeleteBook_ShouldRemove_WhenUserOwnsBook()
        {
            // Arrange
            var context = GetInMemoryDbContext("DeleteBookDB");
            var service = new BookService(context);

            var book = new Book
            {
                Name = "To Delete",
                CreatedBy = "josey"
            };

            context.Books.Add(book);
            context.SaveChanges();

            // Act
            var result = await service.DeleteBook(book.Id, "josey");

            // Assert
            Assert.True(result); // Deletion successful
            Assert.Empty(context.Books); // Book removed from DB
        }

        [Fact]
        public async Task DeleteBook_ReturnsFalse_WhenUserNotOwner()
        {
            // Arrange
            var context = GetInMemoryDbContext("DeleteFailDB");
            var service = new BookService(context);

            var book = new Book
            {
                Name = "Do Not Touch",
                CreatedBy = "realowner"
            };

            context.Books.Add(book);
            context.SaveChanges();

            // Act - try deleting as a different user
            var result = await service.DeleteBook(book.Id, "hacker");

            // Assert
            Assert.False(result); // Should fail (unauthorized)
        }

        [Fact]
        public async Task GetBooks_ReturnsBooks_ForUser()
        {
            // Arrange
            var context = GetInMemoryDbContext("GetUserBooksDB");
            var service = new BookService(context);

            // Add 3 books, 2 by "josey", 1 by "someone"
            context.Books.AddRange(
                new Book { Name = "Book 1", CreatedBy = "josey" },
                new Book { Name = "Book 2", CreatedBy = "josey" },
                new Book { Name = "Not Yours", CreatedBy = "someone" }
            );

            context.SaveChanges();

            // Act
            var books = await service.GetBooks("josey");

            // Assert
            Assert.Equal(2, books.Count); // Only 2 books created by "josey"
            Assert.DoesNotContain(books, b => b.CreatedBy != "josey"); // None from others
        }
    }
}
