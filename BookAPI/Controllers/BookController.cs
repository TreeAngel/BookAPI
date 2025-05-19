using BookAPI.Entities;
using BookAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookDbContext context;

        public BookController(BookDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(string? genre)
        {
            try
            {
                List<BookDto> data = [];
                var bookGenre = !string.IsNullOrEmpty(genre) ? await context.BookGenres.Where(x => x.Genre.Name.ToLower() == genre.Trim().ToLower()).Select(x => new {x.Book, x.Genre}).ToListAsync() : await context.BookGenres.Where(x => x.Book.DeletedAt == null).Select(x => new { x.Book, x.Genre }).ToListAsync();
                foreach (var item in bookGenre)
                {
                    var book = await context.Books.FirstOrDefaultAsync(x => x.Id == item.Book.Id && x.DeletedAt == null);
                    if (book != null)
                    {
                        var dto = book.Adapt<BookDto>();
                        var existAtData = data.FirstOrDefault(x => x.Id == book.Id);
                        var genres = await context.BookGenres.Where(x => x.BookId == book.Id).ToListAsync();
                        if (existAtData != null)
                        {
                            if (!existAtData.Genres.Any(x => x == item.Genre.Name))
                            {
                                existAtData.Genres.Add(item.Genre.Name);
                            }
                        }
                        else
                        {
                            book.Genres.Add(item.Genre.Name);
                            data.Add(book);
                        }
                    }
                }
                //var books = string.IsNullOrEmpty(genre)
                //    ? await context.BookGenres.Where(x => x.Book.DeletedAt == null).Select(x => new { book = x.Book, genre = x.Genre }).ToListAsync()
                //    : await context.BookGenres.Where(x => x.Genre.Name.ToLower() == genre.Trim().ToLower() && x.Book.DeletedAt == null).Select(x => new { book = x.Book, genre = x.Genre }).ToListAsync();
                //foreach (var item in books)
                //{
                //    var book = item.book.Adapt<BookDto>();
                //    var existAtData = data.FirstOrDefault(x => x.Id == item.book.Id);
                //    if (existAtData != null)
                //    {
                //        if (!existAtData.Genres.Any(x => x == item.genre.Name))
                //        {
                //            existAtData.Genres.Add(item.genre.Name);
                //        }
                //    }
                //    else
                //    {
                //        book.Genres.Add(item.genre.Name);
                //        data.Add(book);
                //    }
                //}
                return Ok(new
                {
                    data
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBook(int bookId)
        {
            try
            {
                var book = await context.Books.FirstOrDefaultAsync(x => x.Id == bookId && x.DeletedAt == null);
                if (book is null)
                {
                    return NotFound(new
                    {
                        status = "Failed",
                        message = "Book not found"
                    });
                }
                return Ok(new
                {
                    data = book.Adapt<BookDto>()
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}
