using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using API.Filter;
using Models;
using System.Diagnostics;
using Services.Helpers;


[ApiController]
[Route("api/[controller]")]
[ValidateUserBookIds]
public class BookRentalController : ControllerBase
{
    private readonly IBookRentalService _bookRentalService;

    private readonly IActivityLogger logger;

    public BookRentalController(IBookRentalService bookService, IActivityLogger _logger)
    {
        _bookRentalService = bookService;
        logger = _logger;
    }

    [HttpGet("search")]

    public async Task<IActionResult> SearchBooks(string? title, string? genre)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            var books = await _bookRentalService.SearchBooksAsync(title, genre);
            await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
            return Ok(books);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("rent")]
    public async Task<IActionResult> RentBook(int bookId, int userId)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            if (bookId == 0)
            {
                return BadRequest("Valid book id is required.");
            }
            else if (userId == 0)
            {
                return BadRequest("Valid user id is required.");
            }
            await _bookRentalService.RentBookAsync(bookId, userId);
           // await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
            //await logger.LogActivityAsync("Info", "Rental completed successfully for book id:" + bookId, "/api/bookrental/rent", userId);
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
    public async Task<IActionResult> GetMostOverdueBook()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            var result = await _bookRentalService.GetMostOverdueBookAsync();

            if (result == null)
            {
                return NotFound("No overdue books found.");
            }
            await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
            return Ok(result);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/most-overdue-book");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("least-popular-book")]
    public async Task<IActionResult> GetLeastPopularBook()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            var result = await _bookRentalService.GetLeastPopularBookAsync();

            if (result == null)
            {
                return NotFound("No least popular books found.");
            }
            await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
            return Ok(result);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/most-overdue-book");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("most-popular-book")]
    public async Task<IActionResult> GetMostPopularBook()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            var result = await _bookRentalService.GetMostPopularBookAsync();

            if (result == null)
            {
                return NotFound("No most popular books found.");
            }
            await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
            return Ok(result);
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/most-overdue-book");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("history/user/{userId}")]
    public async Task<IActionResult> GetRentalHistoryByUserId(int userId)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            var rentalHistory = await _bookRentalService.GetRentalHistoryByUserIdAsync(userId);
            if (rentalHistory == null || !rentalHistory.Any())
            {
                return NotFound("No rental history found for the specified user.");
            }
            await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
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
    public async Task<IActionResult> GetRentalHistoryByBookId(int bookId)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            var rentalHistory = await _bookRentalService.GetRentalHistoryByBookIdAsync(bookId);
            if (rentalHistory == null || !rentalHistory.Any())
            {
                return NotFound("No rental history found for the specified book.");
            }
            await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
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
    public async Task<IActionResult> ReturnBook(int bookId, int userId)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            if (bookId == 0)
            {
                return BadRequest("Valid book id is required.");
            }
            else if (userId == 0)
            {
                return BadRequest("Valid user id is required.");
            }
            await _bookRentalService.ReturnBookAsync(bookId, userId);
            await LogPerformanceMetrics(HttpContext.Request.Path, stopwatch, logger);
            await logger.LogActivityAsync("Info", "Return completed successfully for book id:" + bookId, "/api/bookrental/return", userId);
            return Ok("Book returned succesfully");
        }
        catch (Exception ex)
        {
            await logger.LogActivityAsync("Error", ex.Message, "/api/bookrental/return");
            return BadRequest(ex.Message);
        }
    }

    // Common method to log performance metrics
    protected async Task LogPerformanceMetrics(string endpoint, Stopwatch stopwatch, IActivityLogger logger)
    {
        // Log performance metrics
        var log = new ActivityLog
        {
            LogType = "Performance",
            Message = $"Endpoint {endpoint} executed.",
            Endpoint = endpoint,
            DurationMs = (int)stopwatch.ElapsedMilliseconds,
            LogTime = DateTime.UtcNow
        };

        // Add the log to the database and save changes
        await logger.AddActivityAsync(log);
    }
}


