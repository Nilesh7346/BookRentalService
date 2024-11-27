using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using API.Filter;
using Services.Helpers;
using Microsoft.IdentityModel.Tokens;


[ApiController]
[Route("api/[controller]")]
[ValidateUserBookIds]
public class BookRentalController : ControllerBase
{
    private readonly IBookRentalService _bookRentalService;

    public BookRentalController(IBookRentalService bookService)
    {
        _bookRentalService = bookService;
    }

    [HttpGet("search")]

    public async Task<IActionResult> SearchBooks(string? title, string? genre)
    {
        try
        {
            var books = await _bookRentalService.SearchBooksAsync(title, genre);
            return Ok(books);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("rent")]
    public async Task<IActionResult> RentBook(int bookId, int userId, [FromServices] ActivityLogger logger)
    {
        try
        {
            if (bookId == 0)
            {
                return BadRequest("Valid book id is required.");
            }
            else if (userId == 0)
            {
                return BadRequest("Valid user id is required.");
            }
            await _bookRentalService.RentBookAsync(bookId, userId);
            await logger.LogActivityAsync("Info", "Rental completed successfully for book id:" + bookId, "/api/bookrental/rent", userId);
            return Ok("Book rented successfully.");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrentalrental/rent", userId);

            // Handle concurrency conflict
            return Conflict("Concurrency conflict occurred. Please retry.");
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/rent", userId);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("most-overdue-book")]
    public async Task<IActionResult> GetMostOverdueBook([FromServices] ActivityLogger logger)
    {
        try
        {
            var result = await _bookRentalService.GetMostOverdueBookAsync();

            if (result == null)
            {
                return NotFound("No overdue books found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/most-overdue-book");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("least-popular-book")]
    public async Task<IActionResult> GetLeastPopularBook([FromServices] ActivityLogger logger)
    {
        try
        {
            var result = await _bookRentalService.GetLeastPopularBookAsync();

            if (result == null)
            {
                return NotFound("No least popular books found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/most-overdue-book");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("most-popular-book")]
    public async Task<IActionResult> GetMostPopularBook([FromServices] ActivityLogger logger)
    {
        try
        {
            var result = await _bookRentalService.GetMostPopularBookAsync();

            if (result == null)
            {
                return NotFound("No most popular books found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/most-overdue-book");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("history/user/{userId}")]
    public async Task<IActionResult> GetRentalHistoryByUserId(int userId, [FromServices] ActivityLogger logger)
    {
        try
        {
            var rentalHistory = await _bookRentalService.GetRentalHistoryByUserIdAsync(userId);
            if (rentalHistory == null || !rentalHistory.Any())
            {
                return NotFound("No rental history found for the specified user.");
            }

            return Ok(rentalHistory);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/history/user");
            return BadRequest(ex.Message);
        }
    }

    // Endpoint to get rental history by bookId
    [HttpGet("history/book/{bookId}")]
    public async Task<IActionResult> GetRentalHistoryByBookId(int bookId, [FromServices] ActivityLogger logger)
    {
        try
        {
            var rentalHistory = await _bookRentalService.GetRentalHistoryByBookIdAsync(bookId);
            if (rentalHistory == null || !rentalHistory.Any())
            {
                return NotFound("No rental history found for the specified book.");
            }

            return Ok(rentalHistory);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/history/book/");
            return BadRequest(ex.Message);
        }
    }

    // Endpoint to get rental history by bookId
    [HttpPatch("return")]
    public async Task<IActionResult> ReturnBook(int bookId, int userId, [FromServices] ActivityLogger logger)
    {
        try
        {
            if (bookId == 0)
            {
                return BadRequest("Valid book id is required.");
            }
            else if (userId == 0)
            {
                return BadRequest("Valid user id is required.");
            }
            await _bookRentalService.ReturnBookAsync(bookId, userId);
            await logger.LogActivityAsync("Info", "Return completed successfully for book id:" + bookId, "/api/bookrental/return", userId);
            return Ok("Book returned succesfully");
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/return");
            return BadRequest(ex.Message);
        }
    }

}
