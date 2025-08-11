-- This script ensures the Favorites table exists and has at least one record
USE ClosetAppDB2;

-- Create the table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Favorites')
BEGIN
    CREATE TABLE Favorites (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserEmail NVARCHAR(100) NOT NULL,
        ItemType NVARCHAR(50) NOT NULL,
        ItemName NVARCHAR(100) NOT NULL,
        ItemImageUrl NVARCHAR(MAX) NOT NULL
    );
END

-- Add a test favorite if none exist
IF NOT EXISTS (SELECT * FROM Favorites)
BEGIN
    INSERT INTO Favorites (UserEmail, ItemType, ItemName, ItemImageUrl)
    VALUES ('i@gmail.com', 'Fitness', 'Sample Exercise', 'https://via.placeholder.com/150');
END

-- Check if your email has any favorites
IF NOT EXISTS (SELECT * FROM Favorites WHERE UserEmail = 'i@gmail.com')
BEGIN
    INSERT INTO Favorites (UserEmail, ItemType, ItemName, ItemImageUrl)
    VALUES ('i@gmail.com', 'Fitness', 'Database Test', 'https://via.placeholder.com/150');
END

-- Copy any favorites that might have been stored with wrong email casing
-- For example, if records exist with 'I@gmail.com' but your app uses 'i@gmail.com'
IF EXISTS (
    SELECT * FROM Favorites 
    WHERE LOWER(UserEmail) = 'i@gmail.com' 
    AND UserEmail <> 'i@gmail.com'
)
BEGIN
    INSERT INTO Favorites (UserEmail, ItemType, ItemName, ItemImageUrl)
    SELECT 'i@gmail.com', ItemType, ItemName, ItemImageUrl
    FROM Favorites
    WHERE LOWER(UserEmail) = 'i@gmail.com' 
    AND UserEmail <> 'i@gmail.com';
END

-- Show all favorites for your email
SELECT * FROM Favorites WHERE UserEmail = 'i@gmail.com';
