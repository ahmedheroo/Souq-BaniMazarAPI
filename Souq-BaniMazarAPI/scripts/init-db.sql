-- Wait for SQL Server to start
PRINT 'Initializing database...';

-- Create database if it doesn't exist
IF NOT EXISTS(SELECT name FROM master.dbo.sysdatabases WHERE name = 'BaniMazarMarketplaceDb')
BEGIN
    CREATE DATABASE BaniMazarMarketplaceDb;
    PRINT 'Database BaniMazarMarketplaceDb created.';
END
ELSE
BEGIN
    PRINT 'Database BaniMazarMarketplaceDb already exists.';
END
GO

USE BaniMazarMarketplaceDb;
GO

PRINT 'Database initialization completed.';