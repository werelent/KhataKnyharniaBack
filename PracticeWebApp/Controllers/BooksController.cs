using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWebApp.Models;

namespace PracticeWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly DataContext _context;

        public BooksController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        [HttpGet("id")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = _context.Books.Find(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public ActionResult<Book> CreateBook(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books.Find(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
