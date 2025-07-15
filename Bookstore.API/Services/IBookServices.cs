using BookStore.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.API.Services
{
    // Defines the contract for book-related business logic
    public interface IBookService
    {
        // Adds a new book and returns the added book
        Task<Book> AddBook(Book book);

        // Retrieves all books created by a specific user
        Task<List<Book>> GetBooks(string username);

        // Retrieves a single book by its ID
        Task<Book?> GetBookById(int id);

        // Updates an existing book if the user owns it
        Task<Book?> UpdateBook(int id, Book updatedBook, string username);

        // Deletes a book if the user owns it
        Task<bool> DeleteBook(int id, string username);
    }
}
