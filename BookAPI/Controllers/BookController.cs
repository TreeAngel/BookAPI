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
        public async Task<IActionResult> GetBooks(string? genreName)
        {
            try
            {
                if (string.IsNullOrEmpty(genreName))
                {
                    var bookGenres = await context.BookGenres
                        .Include(x => x.Book)
                         .Where(x => x.Book.DeletedAt == null)
                         .Select(x => new { x.Book, x.GenreId })
                       .ToListAsync();
                    List<BookDto> data = new List<BookDto>();
                    foreach (var item in bookGenres)
                    {
                        var book = item.Book.Adapt<BookDto>();
                        var exist = data.FirstOrDefault(x => x.Id == book.Id);
                        var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == item.GenreId);
                        if (exist != null)
                        {
                            if (genre != null && !exist.Genres.Contains(genre.Name))
                            {
                                exist.Genres.Add(genre.Name);
                            }
                        }
                        else
                        {
                            if (genre != null && !book.Genres.Contains(genre.Name))
                            {
                                book.Genres.Add(genre.Name);
                            }
                            data.Add(book);
                        }
                    }
                    return Ok(new
                    {
                        data
                    });
                }
                else
                {
                    var genre = await context.Genres.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == genreName.Trim().ToLower());
                    if (genre is null)
                    {
                        return NotFound(new
                        {
                            status = "Failed",
                            message = "Genre not found"
                        });
                    }
                    var bookGenres = await context.BookGenres
                        .Include(x => x.Book)
                         .Where(x => x.Book.DeletedAt == null && x.GenreId == genre.Id)
                         .Select(x => new { x.Book, x.GenreId })
                       .ToListAsync();
                    List<BookDto> data = new List<BookDto>();
                    foreach (var item in bookGenres)
                    {
                        var book = item.Book.Adapt<BookDto>();
                        var exist = data.FirstOrDefault(x => x.Id == book.Id);
                        if (exist != null)
                        {
                            if (genre != null && !exist.Genres.Contains(genre.Name))
                            {
                                exist.Genres.Add(genre.Name);
                            }
                        }
                        else
                        {
                            if (genre != null && !book.Genres.Contains(genre.Name))
                            {
                                book.Genres.Add(genre.Name);
                            }
                            data.Add(book);
                        }
                    }
                    return Ok(new
                    {
                        data
                    });
                }
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
                var data = new BookDto();
                var bookGenres = await context.BookGenres
                        .Include(x => x.Book)
                         .Where(x => x.Book.DeletedAt == null && x.BookId == bookId)
                         .Select(x => new { x.Book, x.GenreId })
                       .ToListAsync();
                data = bookGenres.First().Book.Adapt<BookDto>();
                foreach (var item in bookGenres)
                {
                    var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == item.GenreId);
                    if (genre != null && !data.Genres.Contains(genre.Name))
                    {
                        data.Genres.Add(genre.Name);
                    }
                }
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
    }
}
