using BookStore.API.Data;
using BookStore.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Services
{
    public class BookService : IBookService
    {
        private readonly BookStoreContext _context;

        // Inject DB context
        public BookService(BookStoreContext context)
        {
            _context = context;
        }

        // Add a new book
        public async Task<Book> AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        // Get all books created by a specific user
        public async Task<List<Book>> GetBooks(string username)
        {
            return await _context.Books
                .Where(b => b.CreatedBy == username)
                .ToListAsync();
        }

        // Get a single book by ID
        public async Task<Book?> GetBookById(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        // Update a book if the user is the creator
        public async Task<Book?> UpdateBook(int id, Book updatedBook, string username)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.CreatedBy != username)
                return null;

            // Update book fields
            book.Name = updatedBook.Name;
            book.Category = updatedBook.Category;
            book.Price = updatedBook.Price;
            book.Description = updatedBook.Description;

            await _context.SaveChangesAsync();
            return book;
        }

        // Delete a book if the user is the creator
        public async Task<bool> DeleteBook(int id, string username)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.CreatedBy != username)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
