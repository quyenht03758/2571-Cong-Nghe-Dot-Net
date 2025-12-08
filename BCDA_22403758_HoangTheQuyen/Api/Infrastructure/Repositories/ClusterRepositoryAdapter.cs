using Dapper;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Infrastructure;

namespace K8sManager.Api.Infrastructure.Repositories;

public class ClusterRepositoryAdapter : IClusterRepository
{
    private readonly DapperConnectionFactory _dbFactory;

    public ClusterRepositoryAdapter(DapperConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<int> CreateAsync(ClusterConfig cluster)
    {
        using var conn = _dbFactory.Create();
        
        var sql = @"
            INSERT INTO ClusterConfig (Name, KubeconfigPath, ContextName, IsDefault, Environment, Description, AddedBy, CreatedAt, UpdatedAt)
            VALUES (@Name, @KubeconfigPath, @ContextName, @IsDefault, @Environment, @Description, @AddedBy, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            cluster.Name,
            cluster.KubeconfigPath,
            cluster.ContextName,
            cluster.IsDefault,
            cluster.Environment,
            cluster.Description,
            cluster.AddedBy,
            cluster.CreatedAt,
            cluster.UpdatedAt
        }).ConfigureAwait(false);

        // If this is set as default, unset all others
        if (cluster.IsDefault)
        {
            await conn.ExecuteAsync(
                "UPDATE ClusterConfig SET IsDefault = 0 WHERE Id != @Id",
                new { Id = id }).ConfigureAwait(false);
        }

        return id;
    }

    public Task<int> AddAsync(ClusterConfig cluster) => CreateAsync(cluster);

    public async Task<IEnumerable<ClusterConfig>> GetAllAsync()
    {
        using var conn = _dbFactory.Create();
        
        var sql = @"
            SELECT c.*, u.Username AS AddedByUsername
            FROM ClusterConfig c
            LEFT JOIN AppUser u ON c.AddedBy = u.Id
            ORDER BY c.IsDefault DESC, c.Name";

        return await conn.QueryAsync<ClusterConfig>(sql).ConfigureAwait(false);
    }

    public async Task<ClusterConfig?> GetByIdAsync(int id)
    {
        using var conn = _dbFactory.Create();
        
        var sql = @"
            SELECT c.*, u.Username AS AddedByUsername
            FROM ClusterConfig c
            LEFT JOIN AppUser u ON c.AddedBy = u.Id
            WHERE c.Id = @Id";

        return await conn.QueryFirstOrDefaultAsync<ClusterConfig>(sql, new { Id = id }).ConfigureAwait(false);
    }

    public async Task<ClusterConfig?> GetDefaultAsync()
    {
        using var conn = _dbFactory.Create();
        
        var sql = @"
            SELECT c.*, u.Username AS AddedByUsername
            FROM ClusterConfig c
            LEFT JOIN AppUser u ON c.AddedBy = u.Id
            WHERE c.IsDefault = 1";

        return await conn.QueryFirstOrDefaultAsync<ClusterConfig>(sql).ConfigureAwait(false);
    }

    public async Task<bool> UpdateAsync(ClusterConfig cluster)
    {
        using var conn = _dbFactory.Create();
        
        var sql = @"
            UPDATE ClusterConfig
            SET Name = @Name,
                KubeconfigPath = @KubeconfigPath,
                ContextName = @ContextName,
                IsDefault = @IsDefault,
                Environment = @Environment,
                Description = @Description,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        var rowsAffected = await conn.ExecuteAsync(sql, new
        {
            cluster.Id,
            cluster.Name,
            cluster.KubeconfigPath,
            cluster.ContextName,
            cluster.IsDefault,
            cluster.Environment,
            cluster.Description,
            cluster.UpdatedAt
        }).ConfigureAwait(false);

        // If this is set as default, unset all others
        if (cluster.IsDefault)
        {
            await conn.ExecuteAsync(
                "UPDATE ClusterConfig SET IsDefault = 0 WHERE Id != @Id",
                new { cluster.Id }).ConfigureAwait(false);
        }

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = _dbFactory.Create();
        
        var sql = "DELETE FROM ClusterConfig WHERE Id = @Id";
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<bool> SetDefaultAsync(int id)
    {
        using var conn = _dbFactory.Create();
        
        // Unset all defaults
        await conn.ExecuteAsync("UPDATE ClusterConfig SET IsDefault = 0").ConfigureAwait(false);
        
        // Set the specified cluster as default
        var rowsAffected = await conn.ExecuteAsync(
            "UPDATE ClusterConfig SET IsDefault = 1, UpdatedAt = @Now WHERE Id = @Id",
            new { Id = id, Now = DateTime.UtcNow }).ConfigureAwait(false);
        return rowsAffected > 0;
    }
}
