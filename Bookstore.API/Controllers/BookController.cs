using Microsoft.AspNetCore.Mvc;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using BookStore.API.Services;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all book routes
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/book - Get all books for current user
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User identity not found");

            var books = await _bookService.GetBooks(username);
            return Ok(books);
        }

        // GET: api/book/{id} - Get a specific book
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookService.GetBookById(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST: api/book - Add a new book
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User identity not found");

            // Set creator and save
            book.CreatedBy = username;
            var addedBook = await _bookService.AddBook(book);
            return CreatedAtAction(nameof(GetBook), new { id = addedBook.Id }, addedBook);
        }

        // PUT: api/book/{id} - Update a book
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = await _bookService.UpdateBook(id, updatedBook, username);
            if (result == null)
                return Forbid("You cannot edit a book you didn’t create or book not found");

            return Ok(result);
        }

        // DELETE: api/book/{id} - Delete a book
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _bookService.DeleteBook(id, username);
            if (!success)
                return Forbid("You cannot delete this book or book not found");

            return Ok("Book deleted");
        }
    }
}
