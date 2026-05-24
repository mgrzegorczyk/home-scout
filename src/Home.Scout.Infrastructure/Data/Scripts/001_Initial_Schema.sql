IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'HomeScout')
    BEGIN
        PRINT 'Creating database HomeScout...';
        CREATE DATABASE HomeScout;
    END
GO

WHILE DB_ID('HomeScout') IS NULL
    BEGIN
        WAITFOR DELAY '00:00:01';
    END
GO

PRINT 'Database HomeScout is ready.';
GO

USE HomeScout;
GO

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'HomeScout')
    BEGIN
        ALTER DATABASE HomeScout SET RECOVERY SIMPLE;
    END
GO

IF OBJECT_ID('RssChannels', 'U') IS NULL
    BEGIN
        PRINT 'Creating table RssChannels...';

        CREATE TABLE RssChannels
        (
            Id          INT PRIMARY KEY IDENTITY (1,1),
            Name        NVARCHAR(255) NOT NULL,
            Url         NVARCHAR(MAX) NOT NULL,
            Description NVARCHAR(MAX),
            CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            UpdatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE()
        );

        PRINT 'Table RssChannels has been created.';

        PRINT 'Creating index IX_RssChannels_Name...';
        CREATE INDEX IX_RssChannels_Name ON RssChannels (Name);
    END
ELSE
    BEGIN
        PRINT 'Table RssChannels already exists.';
    END
GO

IF OBJECT_ID('NewsItems', 'U') IS NULL
    BEGIN
        PRINT 'Creating table NewsItems...';

        CREATE TABLE NewsItems
        (
            Id           INT PRIMARY KEY IDENTITY (1,1),
            RssChannelId INT NOT NULL,
            Title        NVARCHAR(MAX) NOT NULL,
            Description  NVARCHAR(MAX),
            Link         NVARCHAR(MAX),
            PublishedAt  DATETIME2 NOT NULL,
            CreatedAt    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            FOREIGN KEY (RssChannelId)
                REFERENCES RssChannels (Id)
                ON DELETE CASCADE
        );

        PRINT 'Table NewsItems has been created.';

        PRINT 'Creating index IX_NewsItems_RssChannelId...';
        CREATE INDEX IX_NewsItems_RssChannelId ON NewsItems (RssChannelId);

        PRINT 'Creating index IX_NewsItems_PublishedAt...';
        CREATE INDEX IX_NewsItems_PublishedAt ON NewsItems (PublishedAt DESC);
    END
ELSE
    BEGIN
        PRINT 'Table NewsItems already exists.';
    END
GO