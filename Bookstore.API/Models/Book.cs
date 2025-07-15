namespace BookStore.API.Models
{
    // Represents a book record in the database
    public class Book
    {
        public int Id { get; set; }  // Primary Key (auto-increment)

        public string Name { get; set; }  = string.Empty; // Book title

        public string Category { get; set; }  = string.Empty; // Genre or category of the book

        public decimal Price { get; set; }  // Price of the book

        public string Description { get; set; } = string.Empty; // Short description or summary

        public string CreatedBy { get; set; } = string.Empty; // Username of creator (used for access control)
    }
}
