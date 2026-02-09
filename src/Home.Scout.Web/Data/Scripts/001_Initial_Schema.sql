-- Tworzenie bazy danych Home.Scout
USE master;

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'HomeScout')
BEGIN
    ALTER DATABASE HomeScout SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HomeScout;
END

CREATE DATABASE HomeScout;
GO

USE HomeScout;
GO

-- Tabela kanałów RSS
CREATE TABLE RssChannels (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Url NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Tabela elementów newsów
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

-- Indeksy dla wydajności
CREATE INDEX IX_NewsItems_RssChannelId ON NewsItems(RssChannelId);
CREATE INDEX IX_NewsItems_PublishedAt ON NewsItems(PublishedAt DESC);
CREATE INDEX IX_RssChannels_Name ON RssChannels(Name);

GO

