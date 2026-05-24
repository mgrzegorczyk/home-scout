using Dapper;
using Home.Scout.Application.RssChannels;
using Home.Scout.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Home.Scout.Infrastructure.RssChannels;

public class RssChannelRepository : IRssChannelRepository
{
    private readonly string _connectionString;

    public RssChannelRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<List<RssChannel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = """
                               SELECT Id, Name, Url, Description, CreatedAt, UpdatedAt
                               FROM RssChannels
                               ORDER BY Name
                           """;

        var result = await connection.QueryAsync<RssChannel>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return result.AsList();
    }

    public async Task<RssChannel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = """
                               SELECT Id, Name, Url, Description, CreatedAt, UpdatedAt
                               FROM RssChannels
                               WHERE Id = @Id
                           """;

        return await connection.QueryFirstOrDefaultAsync<RssChannel>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(RssChannel channel, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = """
                               INSERT INTO RssChannels (Name, Url, Description, CreatedAt, UpdatedAt)
                               VALUES (@Name, @Url, @Description, @CreatedAt, @UpdatedAt);

                               SELECT CAST(SCOPE_IDENTITY() as int);
                           """;

        var id = await connection.QuerySingleAsync<int>(
            new CommandDefinition(sql, channel, cancellationToken: cancellationToken));

        channel.Id = id;
    }

    public async Task UpdateAsync(RssChannel channel, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = """
                               UPDATE RssChannels
                               SET Name = @Name,
                                   Url = @Url,
                                   Description = @Description,
                                   UpdatedAt = @UpdatedAt
                               WHERE Id = @Id
                           """;

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, channel, cancellationToken: cancellationToken));

        if (affected == 0)
            throw new InvalidOperationException($"Channel with Id {channel.Id} not found.");
    }

    public async Task DeleteAsync(RssChannel channel, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = """
                               DELETE FROM RssChannels
                               WHERE Id = @Id
                           """;

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { channel.Id }, cancellationToken: cancellationToken));

        if (affected == 0)
            throw new InvalidOperationException($"Channel with Id {channel.Id} not found.");
    }
}