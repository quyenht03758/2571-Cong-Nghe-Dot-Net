using Dapper;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Infrastructure;

namespace K8sManager.Api.Infrastructure.Repositories;

public class AppSettingRepositoryAdapter : IAppSettingRepository
{
    private readonly DapperConnectionFactory _dbFactory;

    public AppSettingRepositoryAdapter(DapperConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<AppSetting?> GetByKeyAsync(string key)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT s.*, u.Username AS UpdatedByUsername
            FROM AppSetting s
            LEFT JOIN AppUser u ON s.UpdatedBy = u.Id
            WHERE s.[Key] = @Key";
        var row = await conn.QuerySingleOrDefaultAsync<AppSettingDto>(sql, new { Key = key }).ConfigureAwait(false);
        return row == null ? null : MapToDomain(row);
    }

    public async Task<IEnumerable<AppSetting>> GetAllAsync()
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT s.*, u.Username AS UpdatedByUsername
            FROM AppSetting s
            LEFT JOIN AppUser u ON s.UpdatedBy = u.Id
            ORDER BY s.Category, s.[Key]";
        var rows = await conn.QueryAsync<AppSettingDto>(sql).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<AppSetting>> GetByCategoryAsync(string category)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT s.*, u.Username AS UpdatedByUsername
            FROM AppSetting s
            LEFT JOIN AppUser u ON s.UpdatedBy = u.Id
            WHERE s.Category = @Category
            ORDER BY s.[Key]";
        var rows = await conn.QueryAsync<AppSettingDto>(sql, new { Category = category }).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<bool> UpsertAsync(AppSetting setting)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            MERGE AppSetting AS target
            USING (SELECT @Key AS [Key]) AS source
            ON target.[Key] = source.[Key]
            WHEN MATCHED THEN
                UPDATE SET
                    Value = @Value,
                    Category = @Category,
                    Description = @Description,
                    IsEncrypted = @IsEncrypted,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
            WHEN NOT MATCHED THEN
                INSERT ([Key], Value, Category, Description, IsEncrypted, UpdatedBy, UpdatedAt)
                VALUES (@Key, @Value, @Category, @Description, @IsEncrypted, @UpdatedBy, @UpdatedAt);";

        var rowsAffected = await conn.ExecuteAsync(sql, new
        {
            setting.Key,
            setting.Value,
            setting.Category,
            setting.Description,
            setting.IsEncrypted,
            setting.UpdatedBy,
            setting.UpdatedAt
        }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        using var conn = _dbFactory.Create();
        var rowsAffected = await conn.ExecuteAsync("DELETE FROM AppSetting WHERE [Key] = @Key", new { Key = key }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<int> CreateAsync(AppSetting setting)
    {
        await UpsertAsync(setting).ConfigureAwait(false);
        return 1;
    }

    private static AppSetting MapToDomain(AppSettingDto dto)
    {
        return new AppSetting
        {
            Key = dto.Key,
            Value = dto.Value,
            Category = dto.Category,
            Description = dto.Description,
            IsEncrypted = dto.IsEncrypted,
            UpdatedBy = dto.UpdatedBy,
            UpdatedAt = dto.UpdatedAt
        };
    }

    private class AppSettingDto
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public bool IsEncrypted { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedByUsername { get; set; }
    }
}
