-- Ensure the ClosetAppDB2 database exists
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'ClosetAppDB2')
BEGIN
    CREATE DATABASE ClosetAppDB2;
END
GO

USE ClosetAppDB2;
GO

-- Ensure Users table exists with required structure
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        Username NVARCHAR(50) NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME NOT NULL,
        ProfileImage NVARCHAR(MAX) NULL,
        Preferences NVARCHAR(MAX) NULL,
        FirstName NVARCHAR(100) NULL,
        LastName NVARCHAR(100) NULL,
        Birthdate DATE NULL
    );
    
    -- Insert a default user so we can add items
    INSERT INTO Users (Email, Username, Password, PasswordHash, CreatedAt)
    VALUES ('default@example.com', 'DefaultUser', 'DefaultPassword123!', 'HashedPassword123', GETDATE());
END
GO

-- Ensure ClosetItems table exists with required structure
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
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO

-- Check if we can add the CreatedAt column for backwards compatibility
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ClosetItems]') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE ClosetItems
    ADD CreatedAt DATETIME DEFAULT GETDATE();
END
GO

-- Ensure default user exists (if the table existed but no default user)
IF NOT EXISTS (SELECT * FROM Users WHERE Id = 1)
BEGIN
    INSERT INTO Users (Email, Username, Password, PasswordHash, CreatedAt)
    VALUES ('default@example.com', 'DefaultUser', 'DefaultPassword123!', 'HashedPassword123', GETDATE());
END
GO

-- Display current tables for verification
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
