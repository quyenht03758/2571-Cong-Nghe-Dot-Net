using Dapper;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Infrastructure;

namespace K8sManager.Api.Infrastructure.Repositories;

public class TemplateRepositoryAdapter : ITemplateRepository
{
    private readonly DapperConnectionFactory _dbFactory;

    public TemplateRepositoryAdapter(DapperConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Template?> GetByIdAsync(int id, bool includeVersions = false)
    {
        using var conn = _dbFactory.Create();

        var sql = @"
            SELECT t.*, 
                   cv.YamlContent,
                   u.Username AS CreatedByUsername
            FROM Template t
            LEFT JOIN TemplateVersion cv ON t.Id = cv.TemplateId AND cv.IsCurrent = 1
            LEFT JOIN AppUser u ON t.CreatedBy = u.Id
            WHERE t.Id = @Id";

        var templateDto = await conn.QueryFirstOrDefaultAsync<TemplateDto>(sql, new { Id = id }).ConfigureAwait(false);

        if (templateDto == null) return null;

        var template = MapToDomain(templateDto);

        if (includeVersions)
        {
            var versions = await GetVersionsByTemplateIdAsync(id).ConfigureAwait(false);
            // Versions loaded but not added to Template (no AddVersion method)
            // Template.Versions property is for navigation only
        }

        return template;
    }

    public async Task<(IEnumerable<Template> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? userId = null,
        string? category = null,
        string? searchTerm = null,
        bool? isPublic = null)
    {
        using var conn = _dbFactory.Create();

        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        if (userId.HasValue)
        {
            whereConditions.Add("(t.CreatedBy = @UserId OR t.IsPublic = 1)");
            parameters.Add("UserId", userId.Value);
        }

        if (!string.IsNullOrEmpty(category))
        {
            whereConditions.Add("t.Category = @Category");
            parameters.Add("Category", category);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            whereConditions.Add("(t.Name LIKE @SearchTerm OR t.Description LIKE @SearchTerm OR t.Tags LIKE @SearchTerm)");
            parameters.Add("SearchTerm", $"%{searchTerm}%");
        }

        if (isPublic.HasValue)
        {
            whereConditions.Add("t.IsPublic = @IsPublic");
            parameters.Add("IsPublic", isPublic.Value);
        }

        var whereClause = whereConditions.Any()
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : "";

        // Get total count
        var countSql = $@"
            SELECT COUNT(*)
            FROM Template t
            {whereClause}";

        var totalCount = await conn.ExecuteScalarAsync<int>(countSql, parameters).ConfigureAwait(false);

        // Get paged data
        var offset = (pageNumber - 1) * pageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", pageSize);

        var dataSql = $@"
            SELECT t.*, u.Username AS CreatedByUsername
            FROM Template t
            LEFT JOIN AppUser u ON t.CreatedBy = u.Id
            {whereClause}
            ORDER BY t.UpdatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var rows = await conn.QueryAsync<TemplateDto>(dataSql, parameters).ConfigureAwait(false);
        var items = rows.Select(MapToDomain);

        return (items, totalCount);
    }

    public async Task<int> CreateAsync(Template template)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            INSERT INTO Template (Name, Category, Description, Tags, IsPublic, CreatedBy, CreatedAt, UpdatedAt)
            VALUES (@Name, @Category, @Description, @Tags, @IsPublic, @CreatedBy, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            template.Name,
            template.Category,
            template.Description,
            template.Tags,
            template.IsPublic,
            template.CreatedBy,
            template.CreatedAt,
            template.UpdatedAt
        }).ConfigureAwait(false);

        return id;
    }

    public async Task<bool> UpdateAsync(Template template)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            UPDATE Template
            SET Name = @Name,
                Category = @Category,
                Description = @Description,
                Tags = @Tags,
                IsPublic = @IsPublic,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        var rowsAffected = await conn.ExecuteAsync(sql, new
        {
            template.Id,
            template.Name,
            template.Category,
            template.Description,
            template.Tags,
            template.IsPublic,
            template.UpdatedAt
        }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = _dbFactory.Create();

        // Delete versions first (due to FK constraint)
        await conn.ExecuteAsync("DELETE FROM TemplateVersion WHERE TemplateId = @Id", new { Id = id }).ConfigureAwait(false);

        // Delete template
        var rowsAffected = await conn.ExecuteAsync("DELETE FROM Template WHERE Id = @Id", new { Id = id }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Template>> GetAllAsync(string? category = null, string? search = null)
    {
        using var conn = _dbFactory.Create();
        
        // Query template metadata along with current version's YAML content
        var sql = @"
            SELECT t.*, 
                   cv.YamlContent,
                   u.Username AS CreatedByUsername
            FROM Template t
            LEFT JOIN TemplateVersion cv ON t.Id = cv.TemplateId AND cv.IsCurrent = 1
            LEFT JOIN AppUser u ON t.CreatedBy = u.Id";
        
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(category))
        {
            conditions.Add("t.Category = @Category");
            parameters.Add("Category", category);
        }

        if (!string.IsNullOrEmpty(search))
        {
            conditions.Add("(t.Name LIKE @Search OR t.Description LIKE @Search OR t.Tags LIKE @Search)");
            parameters.Add("Search", $"%{search}%");
        }

        if (conditions.Any())
        {
            sql += " WHERE " + string.Join(" AND ", conditions);
        }

        sql += " ORDER BY t.CreatedAt DESC";

        var rows = await conn.QueryAsync<TemplateDto>(sql, parameters).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<(Template Template, TemplateVersion? CurrentVersion)>> GetAllWithCurrentVersionAsync(string? category = null, string? search = null)
    {
        using var conn = _dbFactory.Create();
        
        var sql = @"
            SELECT t.Id, t.Name, t.Category, t.Description, t.Tags, t.IsPublic, t.CreatedBy, t.CreatedAt, t.UpdatedAt,
                   cv.Id AS VersionId, cv.TemplateId, cv.VersionNumber, cv.YamlContent, cv.ChangeLog, cv.IsCurrent, cv.CreatedBy AS VersionCreatedBy, cv.CreatedAt AS VersionCreatedAt
            FROM Template t
            LEFT JOIN TemplateVersion cv ON t.Id = cv.TemplateId AND cv.IsCurrent = 1";
        
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(category))
        {
            conditions.Add("t.Category = @Category");
            parameters.Add("Category", category);
        }

        if (!string.IsNullOrEmpty(search))
        {
            conditions.Add("(t.Name LIKE @Search OR t.Description LIKE @Search OR t.Tags LIKE @Search)");
            parameters.Add("Search", $"%{search}%");
        }

        if (conditions.Any())
        {
            sql += " WHERE " + string.Join(" AND ", conditions);
        }

        sql += " ORDER BY t.CreatedAt DESC";

        var result = new List<(Template, TemplateVersion?)>();
        var rows = await conn.QueryAsync<TemplateWithVersionDto>(sql, parameters).ConfigureAwait(false);

        foreach (var row in rows)
        {
            var template = new Template
            {
                Id = row.Id,
                Name = row.Name,
                Category = row.Category,
                Description = row.Description,
                Tags = row.Tags,
                IsPublic = row.IsPublic,
                CreatedBy = row.CreatedBy,
                CreatedAt = row.CreatedAt,
                UpdatedAt = row.UpdatedAt
            };

            TemplateVersion? version = null;
            if (row.VersionId.HasValue && row.VersionId.Value > 0)
            {
                version = new TemplateVersion
                {
                    Id = row.VersionId.Value,
                    TemplateId = row.TemplateId ?? 0,
                    VersionNumber = row.VersionNumber ?? 0,
                    YamlContent = row.YamlContent ?? string.Empty,
                    ChangeLog = row.ChangeLog,
                    IsCurrent = row.IsCurrent ?? false,
                    CreatedBy = row.VersionCreatedBy ?? 0,
                    CreatedAt = row.VersionCreatedAt ?? DateTime.UtcNow
                };
            }

            result.Add((template, version));
        }

        return result;
    }

    public async Task<bool> ExistsAsync(string name, int? excludeId = null)
    {
        using var conn = _dbFactory.Create();

        var sql = excludeId.HasValue
            ? "SELECT COUNT(*) FROM Template WHERE Name = @Name AND Id != @ExcludeId"
            : "SELECT COUNT(*) FROM Template WHERE Name = @Name";

        var count = await conn.ExecuteScalarAsync<int>(sql, new { Name = name, ExcludeId = excludeId }).ConfigureAwait(false);
        return count > 0;
    }

    // Template Version operations

    public async Task<TemplateVersion?> GetVersionByIdAsync(int versionId)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT tv.*, u.Username AS CreatedByUsername
            FROM TemplateVersion tv
            LEFT JOIN AppUser u ON tv.CreatedBy = u.Id
            WHERE tv.Id = @Id";

        var dto = await conn.QueryFirstOrDefaultAsync<TemplateVersionDto>(sql, new { Id = versionId }).ConfigureAwait(false);
        return dto == null ? null : MapVersionToDomain(dto);
    }

    public async Task<IEnumerable<TemplateVersion>> GetVersionsByTemplateIdAsync(int templateId)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT tv.*, u.Username AS CreatedByUsername
            FROM TemplateVersion tv
            LEFT JOIN AppUser u ON tv.CreatedBy = u.Id
            WHERE tv.TemplateId = @TemplateId
            ORDER BY tv.VersionNumber DESC";

        var rows = await conn.QueryAsync<TemplateVersionDto>(sql, new { TemplateId = templateId }).ConfigureAwait(false);
        return rows.Select(MapVersionToDomain);
    }

    public async Task<TemplateVersion?> GetCurrentVersionAsync(int templateId)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT tv.*, u.Username AS CreatedByUsername
            FROM TemplateVersion tv
            LEFT JOIN AppUser u ON tv.CreatedBy = u.Id
            WHERE tv.TemplateId = @TemplateId AND tv.IsCurrent = 1";

        var dto = await conn.QueryFirstOrDefaultAsync<TemplateVersionDto>(sql, new { TemplateId = templateId }).ConfigureAwait(false);
        return dto == null ? null : MapVersionToDomain(dto);
    }

    public async Task<int> CreateVersionAsync(TemplateVersion version)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            INSERT INTO TemplateVersion (TemplateId, VersionNumber, YamlContent, ChangeLog, IsCurrent, CreatedBy, CreatedAt)
            VALUES (@TemplateId, @VersionNumber, @YamlContent, @ChangeLog, @IsCurrent, @CreatedBy, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            version.TemplateId,
            version.VersionNumber,
            version.YamlContent,
            version.ChangeLog,
            version.IsCurrent,
            version.CreatedBy,
            version.CreatedAt
        }).ConfigureAwait(false);

        return id;
    }

    public async Task UpdateVersionAsync(TemplateVersion version)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            UPDATE TemplateVersion
            SET IsCurrent = @IsCurrent
            WHERE Id = @Id";

        await conn.ExecuteAsync(sql, new { version.Id, version.IsCurrent }).ConfigureAwait(false);
    }

    public async Task SetCurrentVersionAsync(int templateId, int versionId)
    {
        using var conn = _dbFactory.Create();

        // Unset all current versions for this template
        await conn.ExecuteAsync(
            "UPDATE TemplateVersion SET IsCurrent = 0 WHERE TemplateId = @TemplateId",
            new { TemplateId = templateId }).ConfigureAwait(false);

        // Set the specified version as current
        await conn.ExecuteAsync(
            "UPDATE TemplateVersion SET IsCurrent = 1 WHERE Id = @VersionId",
            new { VersionId = versionId }).ConfigureAwait(false);
    }

    private static Template MapToDomain(TemplateDto dto)
    {
        return new Template
        {
            Id = dto.Id,
            Name = dto.Name,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Category = dto.Category,
            Description = dto.Description,
            Tags = dto.Tags,
            IsPublic = dto.IsPublic
        };
    }

    private static TemplateVersion MapVersionToDomain(TemplateVersionDto dto)
    {
        return new TemplateVersion
        {
            Id = dto.Id,
            TemplateId = dto.TemplateId,
            VersionNumber = dto.VersionNumber,
            YamlContent = dto.YamlContent,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt,
            IsCurrent = dto.IsCurrent,
            ChangeLog = dto.ChangeLog
        };
    }

    private class TemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string? YamlContent { get; set; }  // From current TemplateVersion
        public bool IsPublic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedByUsername { get; set; }
    }

    // Interface implementation aliases
    public Task<int> AddAsync(Template t) => CreateAsync(t);

    private class TemplateVersionDto
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int VersionNumber { get; set; }
        public string YamlContent { get; set; } = null!;
        public string? ChangeLog { get; set; }
        public bool IsCurrent { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUsername { get; set; }
    }

    private class TemplateWithVersionDto
    {
        // Template fields
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public bool IsPublic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Version fields (nullable because of LEFT JOIN)
        public int? VersionId { get; set; }
        public int? TemplateId { get; set; }
        public int? VersionNumber { get; set; }
        public string? YamlContent { get; set; }
        public string? ChangeLog { get; set; }
        public bool? IsCurrent { get; set; }
        public int? VersionCreatedBy { get; set; }
        public DateTime? VersionCreatedAt { get; set; }
    }
}
