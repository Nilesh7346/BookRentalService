-- Step 1: Create the Database
CREATE DATABASE BookRentalDB;
GO

-- Step 2: Use the Database
USE BookRentalDB;
GO

-- Step 3: Create Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE
);

-- Index for faster searching by Name
CREATE NONCLUSTERED INDEX IX_Users_Name
ON Users (Name);
GO

-- Step 4: Create Books Table
CREATE TABLE Books (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(100) NOT NULL,
    ISBN NVARCHAR(13) NOT NULL UNIQUE, -- Unique index on ISBN
    Genre NVARCHAR(50) NOT NULL,
    AvailableCopies INT NOT NULL,
    TotalCopies INT NOT NULL,
    RowVersion ROWVERSION NOT NULL -- Concurrency token
);

-- Index for faster searching by Title and Genre
CREATE NONCLUSTERED INDEX IX_Books_Title_Genre
ON Books (Title, Genre);
GO

-- Step 5: Create Rentals Table
CREATE TABLE Rentals (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RentalDate DATETIME NOT NULL,
    ReturnDate DATETIME NULL,
    IsOverdue BIT NOT NULL DEFAULT 0,
    BookId INT NOT NULL,
    UserId INT NOT NULL,
    RowVersion ROWVERSION NOT NULL, -- Concurrency token
    FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Index for faster lookups by BookId and UserId
CREATE NONCLUSTERED INDEX IX_Rentals_BookId
ON Rentals (BookId);

CREATE NONCLUSTERED INDEX IX_Rentals_UserId
ON Rentals (UserId);

-- Index for finding overdue rentals quickly
CREATE NONCLUSTERED INDEX IX_Rentals_IsOverdue
ON Rentals (IsOverdue);
GO

-- Step 6: Seed Data into Users Table
INSERT INTO Users (Name, Email)
VALUES
    ('John Doe', 'john.doe@example.com'),
    ('Jane Smith', 'jane.smith@example.com');
GO

-- Step 7: Seed Data into Books Table
INSERT INTO Books (Title, Author, ISBN, Genre, AvailableCopies, TotalCopies)
VALUES
    ('The Great Gatsby', 'F. Scott Fitzgerald', '9780743273565', 'Classics', 5, 5),
    ('To Kill a Mockingbird', 'Harper Lee', '9780060935467', 'Classics', 4, 4),
    ('1984', 'George Orwell', '9780451524935', 'Dystopian', 3, 3),
    ('Pride and Prejudice', 'Jane Austen', '9780141199078', 'Romance', 4, 4),
    ('The Catcher in the Rye', 'J.D. Salinger', '9780316769488', 'Classics', 3, 3),
    ('The Hobbit', 'J.R.R. Tolkien', '9780547928227', 'Fantasy', 5, 5),
    ('Fahrenheit 451', 'Ray Bradbury', '9781451673319', 'Science Fiction', 4, 4),
    ('The Book Thief', 'Markus Zusak', '9780375842207', 'Historical Fiction', 3, 3),
    ('Moby-Dick', 'Herman Melville', '9781503280786', 'Classics', 2, 2),
    ('War and Peace', 'Leo Tolstoy', '9781400079988', 'Historical Fiction', 2, 2);
GO

-- Step 8: Create activity log table with for logs and performance

CREATE TABLE ActivityLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LogType NVARCHAR(50) NOT NULL,        -- E.g., "Info", "Error", "Performance"
    Message NVARCHAR(MAX) NOT NULL,      -- Log details
    LogTime DATETIME NOT NULL DEFAULT GETDATE(), -- Timestamp of the log
    Endpoint NVARCHAR(200) NULL,         -- API endpoint that generated the log
    UserId INT NULL,                     -- UserId related to the activity (if applicable)
    DurationMs INT NULL                  -- Execution time in milliseconds (for performance logs)
);
