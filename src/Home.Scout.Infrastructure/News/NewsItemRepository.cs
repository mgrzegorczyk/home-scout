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

    public async Task<IReadOnlyList<NewsItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT Id, RssChannelId, Title, Description, Link, PublishedAt, CreatedAt 
            FROM NewsItems 
            ORDER BY PublishedAt DESC";

        var result = await connection.QueryAsync<NewsItem>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return result.AsList();
    }

    public async Task<IReadOnlyList<NewsItem>> GetByChannelIdAsync(int channelId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT Id, RssChannelId, Title, Description, Link, PublishedAt, CreatedAt 
            FROM NewsItems 
            WHERE RssChannelId = @ChannelId 
            ORDER BY PublishedAt DESC";

        var result = await connection.QueryAsync<NewsItem>(
            new CommandDefinition(sql, new { ChannelId = channelId }, cancellationToken: cancellationToken));

        return result.AsList();
    }

    public async Task<NewsItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT Id, RssChannelId, Title, Description, Link, PublishedAt, CreatedAt 
            FROM NewsItems 
            WHERE Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<NewsItem>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<int> AddAsync(NewsItem entity, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            INSERT INTO NewsItems (RssChannelId, Title, Description, Link, PublishedAt, CreatedAt) 
            VALUES (@RssChannelId, @Title, @Description, @Link, @PublishedAt, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(sql, new
            {
                entity.RssChannelId,
                entity.Title,
                entity.Description,
                entity.Link,
                entity.PublishedAt,
                entity.CreatedAt
            }, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(NewsItem entity, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            UPDATE NewsItems 
            SET RssChannelId = @RssChannelId,
                Title = @Title,
                Description = @Description,
                Link = @Link,
                PublishedAt = @PublishedAt 
            WHERE Id = @Id";

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                entity.Id,
                entity.RssChannelId,
                entity.Title,
                entity.Description,
                entity.Link,
                entity.PublishedAt
            }, cancellationToken: cancellationToken));

        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "DELETE FROM NewsItems WHERE Id = @Id";

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return affected > 0;
    }

    public async Task<bool> DeleteByChannelIdAsync(int channelId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "DELETE FROM NewsItems WHERE RssChannelId = @ChannelId";

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { ChannelId = channelId }, cancellationToken: cancellationToken));

        return affected > 0;
    }

    public async Task BulkInsertIgnoreDuplicatesAsync(
        IEnumerable<NewsItem> entities,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        const string sql = @"
            INSERT INTO NewsItems
            (RssChannelId, Title, Description, Link, PublishedAt, CreatedAt)
            SELECT @RssChannelId, @Title, @Description, @Link, @PublishedAt, @CreatedAt
            WHERE NOT EXISTS (
                SELECT 1 FROM NewsItems
                WHERE RssChannelId = @RssChannelId
                  AND Link = @Link
            );";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entities, transaction, cancellationToken: cancellationToken));

        await transaction.CommitAsync(cancellationToken);
    }
}