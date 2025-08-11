-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ClosetAppDB2')
BEGIN
    CREATE DATABASE ClosetAppDB2;
END
GO

USE ClosetAppDB2;
GO 