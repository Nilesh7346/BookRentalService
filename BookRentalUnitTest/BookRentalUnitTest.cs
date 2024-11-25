namespace BookRentalUnitTest
{
    using NUnit.Framework;
    using Moq;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Services.Helpers;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Data;
    using Microsoft.EntityFrameworkCore;

    [TestFixture]
    public class BookRentalUnitTest
    {
        private Mock<IBookRentalService> _bookRentalServiceMock;
        private Mock<ActivityLogger> _activityLoggerMock;
        private BookRentalController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")  // In-memory DB
            .Options;
            AppDbContext _appDbContext;
            _appDbContext = new AppDbContext(options);

            _bookRentalServiceMock = new Mock<IBookRentalService>();
            _activityLoggerMock = new Mock<ActivityLogger>(_appDbContext);
            _controller = new BookRentalController(_bookRentalServiceMock.Object);
        }



        [Test]
        public async Task SearchBooks_ReturnsOkWithBooks_WhenBooksExist()
        {
            // Arrange
            var mockBooks = new List<BookDto> { new BookDto { ISBN = "9780743273565", Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Classics", AvailableCopies = 5, TotalCopies = 5 },
                                              { new BookDto { ISBN = "9780547928227", Title = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", AvailableCopies = 4, TotalCopies = 4 } } };
            _bookRentalServiceMock.Setup(s => s.SearchBooksAsync("Title", "Genre"))
                                  .ReturnsAsync(mockBooks);

            // Act
            var result = await _controller.SearchBooks("Title", "Genre");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(mockBooks));
        }

        [Test]
        public async Task SearchBooks_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            _bookRentalServiceMock.Setup(s => s.SearchBooksAsync("Title", "Genre"))
                .ThrowsAsync(new System.Exception("Error message"));

            // Act
            var result = await _controller.SearchBooks("Title", "Genre");

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Error message"));

        }

        [Test]
        public async Task RentBook_ReturnsOk_WhenBookIsRented()
        {
            _bookRentalServiceMock.Setup(s => s.RentBookAsync(1, 101))
                      .ReturnsAsync(true);
            var result = await _controller.RentBook(1, 101, _activityLoggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Book rented successfully."));
        }

        [Test]
        public async Task RentBook_ReturnsConflict_WhenConcurrencyExceptionOccurs()
        {
            // Arrange
            _bookRentalServiceMock.Setup(s => s.RentBookAsync(1, 101))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act
            var result = await _controller.RentBook(1, 101, _activityLoggerMock.Object);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.That(conflictResult.Value, Is.EqualTo("Concurrency conflict occurred. Please retry."));
        }

        [Test]
        public async Task GetMostOverdueBook_ReturnsOk_WhenBookExists()
        {
            // Arrange
            var overdueBook = new BookStatisticsDto
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                RentalCount = 1,
                BookId = 1,
            };
            _bookRentalServiceMock.Setup(s => s.GetMostOverdueBookAsync())
                .ReturnsAsync(overdueBook);

            // Act
            var result = await _controller.GetMostOverdueBook(_activityLoggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(overdueBook));
        }

        [Test]
        public async Task GetMostOverdueBook_ReturnsNotFound_WhenNoBookExists()
        {
            // Arrange
            _bookRentalServiceMock.Setup(s => s.GetMostOverdueBookAsync())
                .ReturnsAsync((BookStatisticsDto)null);

            // Act
            var result = await _controller.GetMostOverdueBook(_activityLoggerMock.Object);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.That(notFoundResult.Value, Is.EqualTo("No overdue books found."));
        }

        [Test]
        public async Task GetRentalHistoryByUserId_ReturnsOk_WhenHistoryExists()
        {
            // Arrange
            var history = new List<RentalHistoryDto>
            { new RentalHistoryDto
              { UserName = "TestUser",
                BookName = "TestBook",
                IsOverdue = false,
                RentalDate = DateTime.UtcNow.AddDays(-1),
                RentalId = 1, ReturnDate = DateTime.UtcNow
               }
             };
            _bookRentalServiceMock.Setup(s => s.GetRentalHistoryByUserIdAsync(101))
                .ReturnsAsync(history);

            // Act
            var result = await _controller.GetRentalHistoryByUserId(101, _activityLoggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(history));
        }

        [Test]
        public async Task GetRentalHistoryByUserId_ReturnsNotFound_WhenNoHistoryExists()
        {
            // Arrange
            _bookRentalServiceMock.Setup(s => s.GetRentalHistoryByUserIdAsync(101))
                .ReturnsAsync(new List<RentalHistoryDto>());

            // Act
            var result = await _controller.GetRentalHistoryByUserId(101, _activityLoggerMock.Object);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.EqualTo("No rental history found for the specified user."));
        }

    }

}