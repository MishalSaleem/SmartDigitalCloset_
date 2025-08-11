CREATE DATABASE IF NOT EXISTS ClosetAppDB2;
GO

USE ClosetAppDB2;
GO

-- Create Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        Username NVARCHAR(50) NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        ProfileImage NVARCHAR(MAX) NULL,
        Preferences NVARCHAR(MAX) NULL,
        FirstName NVARCHAR(100) NULL,
        LastName NVARCHAR(100) NULL,
        Birthdate DATE NULL
    );
    
    -- Insert default user if needed
    IF NOT EXISTS (SELECT * FROM Users WHERE Id = 1)
    BEGIN
        INSERT INTO Users (Email, Username, Password, PasswordHash, CreatedAt)
        VALUES ('default@example.com', 'DefaultUser', 'DefaultPassword123!', 'HashedPassword123', GETDATE());
    END
END;
GO

-- Create ClosetItems table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClosetItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE ClosetItems (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Type NVARCHAR(100) NOT NULL,
        Season NVARCHAR(50) NOT NULL,
        Color NVARCHAR(50) NOT NULL,
        ImagePath NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_ClosetItems_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END;
GO

-- Create Outfits table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Outfits]') AND type in (N'U'))
BEGIN
    CREATE TABLE Outfits (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Name NVARCHAR(255) NOT NULL,
        Tags NVARCHAR(255) NULL,
        PlannedDate DATE NOT NULL,
        ItemIds NVARCHAR(MAX) NOT NULL,  -- Stores comma-separated item IDs
        CreatedAt DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_Outfits_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END;
GO

-- List all tables in database
SELECT name FROM sys.tables;
GO

-- List all columns in ClosetItems table
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ClosetItems';
GO

-- List all columns in Outfits table
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Outfits';
GO
