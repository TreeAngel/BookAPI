using BookAPI.Entities;
using BookAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookAPI.Controllers
{
    [Authorize(Roles = "user")]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookDbContext context;

        public UserController(BookDbContext context)
        {
            this.context = context;
        }

        [HttpGet("withlist")]
        public async Task<IActionResult> GetWithlist()
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var data = context.Wishlists.Where(x => x.UserId == userId && x.DeletedAt == null && x.Book.DeletedAt == null).Include(x => x.Book).ToList().Adapt<List<GetWishlistDto>>();
                foreach (var item in data)
                {
                    var bookGenres = await context.BookGenres.Include(x => x.Genre).Where(x => x.BookId == item.Book.Id).ToListAsync();
                    foreach (var item1 in bookGenres)
                    {
                        var genre = await context.Genres.FindAsync(item1.GenreId);
                        if (genre != null && !item.Book.Genres.Contains(genre.Name))
                        {
                            item.Book.Genres.Add(genre.Name);
                        }
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

        [HttpPost("wishlist")]
        public async Task<IActionResult> CreateWishlist([FromBody] int bookId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var wishlist = await context.Wishlists.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
                if (wishlist != null)
                {
                    if (wishlist.DeletedAt != null)
                    {
                        wishlist.DeletedAt = null;
                        context.Wishlists.Update(wishlist);
                        await context.SaveChangesAsync();
                        return Ok(new
                        {
                            status = "Success",
                            message = "Book added to wishlist"
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            status = "Failed",
                            message = "Book already in wishlist"
                        });
                    }
                }
                var book = await context.Books.FindAsync(bookId);
                if (book != null && book.DeletedAt != null)
                {
                    return NotFound(new
                    {
                        status = "Failed",
                        message = "Book not found",
                    });
                }
                wishlist = new Wishlist
                {
                    UserId = userId,
                    BookId = bookId,
                    CreatedAt = DateTime.Now
                };
                await context.Wishlists.AddAsync(wishlist);
                await context.SaveChangesAsync();
                return Ok(new
                {
                    status = "Success",
                    message = "Book added to wishlist"
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpDelete("wishlist/{id}")]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var wishlist = await context.Wishlists.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId && x.DeletedAt == null);
                if (wishlist is null)
                {
                    return NotFound(new
                    {
                        status = "Failed",
                        message = "Wishlist not found"
                    });
                }
                wishlist.DeletedAt = DateTime.Now;
                context.Wishlists.Update(wishlist);
                await context.SaveChangesAsync();
                return Ok(new
                {
                    status = "Success",
                    message = "Wishlist deleted"
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpGet("transaction")]
        public async Task<IActionResult> GetTransactions()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var transactions = await context.Transactions.Where(x => x.UserId == userId).ToListAsync();
                return Ok(new
                {
                    data = transactions.Adapt<List<TransactionDto>>()
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpGet("transaction/{id}")]
        public async Task<IActionResult> GetTransactionDetail(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var transaction = await context.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
                if (transaction == null)
                {
                    return NotFound(new
                    {
                        status = "Failed",
                        message = "Transaction not found"
                    });
                }
                var details = await context.TransactionDetails.Where(x => x.TransactionId == transaction.Id).ToListAsync();
                var data = new TDetailDto
                {
                    Id = transaction.Id,
                    Code = transaction.Code,
                    Subtotal = transaction.Subtotal,
                    TransactionDate = transaction.TransactionDate,
                    TransactionDetails = details.Adapt<List<TransactionDetailDto>>()
                };
                {
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

        [HttpPost("transaction")]
        public async Task<IActionResult> PostTransaction([FromBody] List<CreateTransactionDto> request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                if (request.Count <= 0)
                {
                    return BadRequest(new
                    {
                        status = "Failed",
                        message = "Transaction cannot be empty"
                    });
                }
                if (request.Any(x => x.Qty <= 0))
                {
                    return BadRequest(new
                    {
                        status = "Failed",
                        message = "Transaction quantity must be greater than 0"
                    });
                }
                var transaction = new Transaction
                {
                    Code = $"TR00{context.Transactions.Count() + 1}",
                    UserId = userId,
                    Subtotal = 0,
                    TransactionDate = DateOnly.FromDateTime(DateTime.Now),
                };
                List<TransactionDetail> transactionDetails = new List<TransactionDetail>();
                foreach (var item in request)
                {
                    var book = await context.Books.FirstOrDefaultAsync(x => x.Id == item.BookId && x.DeletedAt == null);
                    if (book is null)
                    {
                        return BadRequest(new
                        {
                            status = "Failed",
                            message = $"Book with id {item.BookId} not found"
                        });
                    }
                    long totalPice = book.Price * item.Qty;
                    transactionDetails.Add(new TransactionDetail
                    {
                        TransactionId = transaction.Id,
                        BookId = item.BookId,
                        Qty = item.Qty,
                        TotalPrice = totalPice
                    });
                }
                transaction.Subtotal = transactionDetails.Sum(x => x.TotalPrice);
                await context.Transactions.AddAsync(transaction);
                await context.TransactionDetails.AddRangeAsync(transactionDetails);
                await context.SaveChangesAsync();
                var data = transaction.Adapt<TransactionDto>();
                data.TransactionDetails = transactionDetails.Adapt<List<TransactionDetailDto>>();
                return Ok(new
                {
                    status = "Success",
                    message = "Transaction created",
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
