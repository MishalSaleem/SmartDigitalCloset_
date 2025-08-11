USE ClosetAppDB2;
GO

-- Drop foreign key constraints first
DECLARE @SQL NVARCHAR(MAX) = '';
SELECT @SQL = @SQL + 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id))
    + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) 
    + ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('Users');
EXEC sp_executesql @SQL;

-- Now drop the Users table if it exists
IF OBJECT_ID('Users', 'U') IS NOT NULL
    DROP TABLE Users;
GO

-- Create Users table with all required columns
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
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO

-- Create Outfits table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Outfits]') AND type in (N'U'))
BEGIN
    CREATE TABLE Outfits (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Name NVARCHAR(255) NOT NULL,
        Tags NVARCHAR(MAX) NULL,
        PlannedDate DATE NOT NULL,
        ItemIds NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO 