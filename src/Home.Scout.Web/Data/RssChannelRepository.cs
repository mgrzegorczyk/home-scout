using Home.Scout.Web.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Home.Scout.Web.Data;

/// <summary>
/// Repozytorium dla kanałów RSS wykorzystujące Dappera
/// </summary>
public class RssChannelRepository : IRssChannelRepository
{
    private readonly string _connectionString;

    public RssChannelRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<List<RssChannel>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT Id, Name, Url, Description, CreatedAt, UpdatedAt FROM RssChannels ORDER BY Name";
        var channels = await connection.QueryAsync<RssChannel>(sql);
        return channels.ToList();
    }

    public async Task<RssChannel?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT Id, Name, Url, Description, CreatedAt, UpdatedAt FROM RssChannels WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<RssChannel>(sql, new { Id = id });
    }

    public async Task<int> AddAsync(RssChannel channel)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            INSERT INTO RssChannels (Name, Url, Description, CreatedAt, UpdatedAt) 
            VALUES (@Name, @Url, @Description, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        return await connection.QuerySingleAsync<int>(sql, new
        {
            channel.Name,
            channel.Url,
            channel.Description,
            channel.CreatedAt,
            channel.UpdatedAt
        });
    }

    public async Task<bool> UpdateAsync(RssChannel channel)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            UPDATE RssChannels 
            SET Name = @Name, Url = @Url, Description = @Description, UpdatedAt = @UpdatedAt 
            WHERE Id = @Id";
        
        var result = await connection.ExecuteAsync(sql, new
        {
            channel.Id,
            channel.Name,
            channel.Url,
            channel.Description,
            channel.UpdatedAt
        });
        
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "DELETE FROM RssChannels WHERE Id = @Id";
        var result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }
}



