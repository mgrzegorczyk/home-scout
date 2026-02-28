using Dapper;
using Home.Scout.Application.News;
using Home.Scout.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Home.Scout.Infrastructure.News;

public class NewsItemRepository : INewsItemRepository
{
    private readonly string _connectionString;

    public NewsItemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<List<NewsItem>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            SELECT Id, RssChannelId, Title, Description, Link, PublishedAt, CreatedAt 
            FROM NewsItems 
            ORDER BY PublishedAt DESC";
        
        var items = await connection.QueryAsync<NewsItem>(sql);
        return items.ToList();
    }

    public async Task<List<NewsItem>> GetByChannelIdAsync(int channelId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            SELECT Id, RssChannelId, Title, Description, Link, PublishedAt, CreatedAt 
            FROM NewsItems 
            WHERE RssChannelId = @ChannelId 
            ORDER BY PublishedAt DESC";
        
        var items = await connection.QueryAsync<NewsItem>(sql, new { ChannelId = channelId });
        return items.ToList();
    }

    public async Task<NewsItem?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            SELECT Id, RssChannelId, Title, Description, Link, PublishedAt, CreatedAt 
            FROM NewsItems 
            WHERE Id = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<NewsItem>(sql, new { Id = id });
    }

    public async Task<int> AddAsync(NewsItem newsItem)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            INSERT INTO NewsItems (RssChannelId, Title, Description, Link, PublishedAt, CreatedAt) 
            VALUES (@RssChannelId, @Title, @Description, @Link, @PublishedAt, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        return await connection.QuerySingleAsync<int>(sql, new
        {
            newsItem.RssChannelId,
            newsItem.Title,
            newsItem.Description,
            newsItem.Link,
            newsItem.PublishedAt,
            newsItem.CreatedAt
        });
    }

    public async Task<bool> UpdateAsync(NewsItem newsItem)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            UPDATE NewsItems 
            SET RssChannelId = @RssChannelId, Title = @Title, Description = @Description, 
                Link = @Link, PublishedAt = @PublishedAt 
            WHERE Id = @Id";
        
        var result = await connection.ExecuteAsync(sql, new
        {
            newsItem.Id,
            newsItem.RssChannelId,
            newsItem.Title,
            newsItem.Description,
            newsItem.Link,
            newsItem.PublishedAt
        });
        
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "DELETE FROM NewsItems WHERE Id = @Id";
        var result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<bool> DeleteByChannelIdAsync(int channelId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "DELETE FROM NewsItems WHERE RssChannelId = @ChannelId";
        var result = await connection.ExecuteAsync(sql, new { ChannelId = channelId });
        return result > 0;
    }
    
    public async Task BulkInsertIgnoreDuplicatesAsync(IEnumerable<NewsItem> items)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        const string sql = @"
        INSERT INTO NewsItems
        (RssChannelId, Title, Description, Link, PublishedAt, CreatedAt)
        SELECT @RssChannelId, @Title, @Description, @Link, @PublishedAt, @CreatedAt
        WHERE NOT EXISTS (
            SELECT 1 FROM NewsItems
            WHERE RssChannelId = @RssChannelId
              AND Link = @Link
        );";

        await connection.ExecuteAsync(sql, items, transaction);

        await transaction.CommitAsync();
    }
}



