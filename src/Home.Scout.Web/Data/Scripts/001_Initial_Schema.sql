IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HomeScout')
    BEGIN
        PRINT 'Database HomeScout does not exist. Creating database...';
        CREATE DATABASE HomeScout;
        PRINT 'Database HomeScout has been created.';
    END
ELSE
    BEGIN
        PRINT 'Database HomeScout already exists.';
    END
GO

USE HomeScout;
GO


IF OBJECT_ID('RssChannels', 'U') IS NULL
    BEGIN
        PRINT 'Creating table RssChannels...';
        CREATE TABLE RssChannels (
                                     Id INT PRIMARY KEY IDENTITY(1,1),
                                     Name NVARCHAR(255) NOT NULL,
                                     Url NVARCHAR(MAX) NOT NULL,
                                     Description NVARCHAR(MAX),
                                     CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                                     UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
        );
        PRINT 'Table RssChannels has been created.';

        PRINT 'Creating index IX_RssChannels_Name...';
        CREATE INDEX IX_RssChannels_Name ON RssChannels(Name);
        PRINT 'Index IX_RssChannels_Name has been created.';
    END
ELSE
    BEGIN
        PRINT 'Table RssChannels already exists.';
    END
GO


IF OBJECT_ID('NewsItems', 'U') IS NULL
    BEGIN
        PRINT 'Creating table NewsItems...';
        CREATE TABLE NewsItems (
                                   Id INT PRIMARY KEY IDENTITY(1,1),
                                   RssChannelId INT NOT NULL,
                                   Title NVARCHAR(MAX) NOT NULL,
                                   Description NVARCHAR(MAX),
                                   Link NVARCHAR(MAX),
                                   PublishedAt DATETIME2 NOT NULL,
                                   CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                                   FOREIGN KEY (RssChannelId) REFERENCES RssChannels(Id) ON DELETE CASCADE
        );
        PRINT 'Table NewsItems has been created.';

        PRINT 'Creating index IX_NewsItems_RssChannelId...';
        CREATE INDEX IX_NewsItems_RssChannelId ON NewsItems(RssChannelId);
        PRINT 'Index IX_NewsItems_RssChannelId has been created.';

        PRINT 'Creating index IX_NewsItems_PublishedAt...';
        CREATE INDEX IX_NewsItems_PublishedAt ON NewsItems(PublishedAt DESC);
        PRINT 'Index IX_NewsItems_PublishedAt has been created.';
    END
ELSE
    BEGIN
        PRINT 'Table NewsItems already exists.';
    END
GO
