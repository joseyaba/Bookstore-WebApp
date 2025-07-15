using Microsoft.EntityFrameworkCore;
using BookStore.API.Models;

namespace BookStore.API.Data
{
    // EF Core database context for the Bookstore application
    public class BookStoreContext : DbContext
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
        {
        }

        // Table for storing books
        public DbSet<Book> Books { get; set; }

        // Table for storing users
        public DbSet<User> Users { get; set; }
    }
}
