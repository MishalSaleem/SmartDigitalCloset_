-- This script adds a CreatedAt timestamp column to the Favorites table
-- Run this script to track when favorites were added

USE ClosetAppDB2;

-- Check if the column already exists
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE name = 'CreatedAt' AND object_id = OBJECT_ID('Favorites')
)
BEGIN
    -- Add the CreatedAt column with default value of current timestamp
    ALTER TABLE Favorites ADD CreatedAt DATETIME DEFAULT GETDATE();
    
    -- Update existing records to have a timestamp (current time)
    UPDATE Favorites SET CreatedAt = GETDATE();
    
    PRINT 'Added CreatedAt column to Favorites table';
END
ELSE
BEGIN
    PRINT 'CreatedAt column already exists in Favorites table';
END

-- Show the updated table structure
SELECT COLUMN_NAME, DATA_TYPE, COLUMN_DEFAULT 
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Favorites'
ORDER BY ORDINAL_POSITION;

-- Show the most recently added favorites
SELECT TOP 10 Id, UserEmail, ItemType, ItemName, ItemImageUrl, CreatedAt
FROM Favorites
ORDER BY CreatedAt DESC;
