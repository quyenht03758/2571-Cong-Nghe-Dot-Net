using Dapper;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Infrastructure;

namespace K8sManager.Api.Infrastructure.Repositories;

public class AuditLogRepositoryAdapter : IAuditLogRepository
{
    private readonly DapperConnectionFactory _dbFactory;

    public AuditLogRepositoryAdapter(DapperConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<AuditLog?> GetByIdAsync(long id)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT a.*, u.Username, c.Name AS ClusterName
            FROM AuditLog a
            LEFT JOIN AppUser u ON a.UserId = u.Id
            LEFT JOIN ClusterConfig c ON a.ClusterId = c.Id
            WHERE a.Id = @Id";

        var row = await conn.QueryFirstOrDefaultAsync<AuditLogDto>(sql, new { Id = id }).ConfigureAwait(false);
        return row == null ? null : MapToDomain(row);
    }

    public async Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? userId = null,
        int? clusterId = null,
        string? action = null,
        bool? success = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        using var conn = _dbFactory.Create();

        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        if (userId.HasValue)
        {
            whereConditions.Add("a.UserId = @UserId");
            parameters.Add("UserId", userId.Value);
        }

        if (clusterId.HasValue)
        {
            whereConditions.Add("a.ClusterId = @ClusterId");
            parameters.Add("ClusterId", clusterId.Value);
        }

        if (!string.IsNullOrEmpty(action))
        {
            whereConditions.Add("a.Action LIKE @Action");
            parameters.Add("Action", $"%{action}%");
        }

        if (success.HasValue)
        {
            whereConditions.Add("a.Success = @Success");
            parameters.Add("Success", success.Value);
        }

        if (fromDate.HasValue)
        {
            whereConditions.Add("a.CreatedAt >= @FromDate");
            parameters.Add("FromDate", fromDate.Value);
        }

        if (toDate.HasValue)
        {
            whereConditions.Add("a.CreatedAt <= @ToDate");
            parameters.Add("ToDate", toDate.Value);
        }

        var whereClause = whereConditions.Any()
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : "";

        // Get total count
        var countSql = $@"
            SELECT COUNT(*)
            FROM AuditLog a
            {whereClause}";

        var totalCount = await conn.ExecuteScalarAsync<int>(countSql, parameters).ConfigureAwait(false);

        // Get paged data
        var offset = (pageNumber - 1) * pageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", pageSize);

        var dataSql = $@"
            SELECT a.*, u.Username, c.Name AS ClusterName
            FROM AuditLog a
            LEFT JOIN AppUser u ON a.UserId = u.Id
            LEFT JOIN ClusterConfig c ON a.ClusterId = c.Id
            {whereClause}
            ORDER BY a.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var rows = await conn.QueryAsync<AuditLogDto>(dataSql, parameters).ConfigureAwait(false);
        var items = rows.Select(MapToDomain);

        return (items, totalCount);
    }

    public async Task<long> CreateAsync(AuditLog auditLog)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            INSERT INTO AuditLog (UserId, ClusterId, Action, ResourceKind, ResourceName,
                                  Namespace, Success, ErrorMessage, RequestPayload,
                                  ResponseData, IpAddress, Duration, CreatedAt)
            VALUES (@UserId, @ClusterId, @Action, @ResourceKind, @ResourceName,
                    @Namespace, @Success, @ErrorMessage, @RequestPayload,
                    @ResponseData, @IpAddress, @Duration, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS bigint);";

        var id = await conn.ExecuteScalarAsync<long>(sql, new
        {
            auditLog.UserId,
            auditLog.ClusterId,
            auditLog.Action,
            auditLog.ResourceKind,
            auditLog.ResourceName,
            auditLog.Namespace,
            auditLog.Success,
            auditLog.ErrorMessage,
            auditLog.RequestPayload,
            auditLog.ResponseData,
            auditLog.IpAddress,
            auditLog.Duration,
            auditLog.CreatedAt
        }).ConfigureAwait(false);

        return id;
    }

    public async Task<IEnumerable<AuditLog>> GetRecentByUserAsync(int userId, int count = 10)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT TOP (@Count) a.*, u.Username, c.Name AS ClusterName
            FROM AuditLog a
            LEFT JOIN AppUser u ON a.UserId = u.Id
            LEFT JOIN ClusterConfig c ON a.ClusterId = c.Id
            WHERE a.UserId = @UserId
            ORDER BY a.CreatedAt DESC";

        var rows = await conn.QueryAsync<AuditLogDto>(sql, new { UserId = userId, Count = count }).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<AuditLog>> GetFailedActionsAsync(int? userId = null, DateTime? since = null, int limit = 100)
    {
        using var conn = _dbFactory.Create();

        var whereConditions = new List<string> { "a.Success = 0" };
        var parameters = new DynamicParameters();
        parameters.Add("Limit", limit);

        if (userId.HasValue)
        {
            whereConditions.Add("a.UserId = @UserId");
            parameters.Add("UserId", userId.Value);
        }

        if (since.HasValue)
        {
            whereConditions.Add("a.CreatedAt >= @Since");
            parameters.Add("Since", since.Value);
        }

        var whereClause = string.Join(" AND ", whereConditions);

        var sql = $@"
            SELECT TOP (@Limit) a.*, u.Username, c.Name AS ClusterName
            FROM AuditLog a
            LEFT JOIN AppUser u ON a.UserId = u.Id
            LEFT JOIN ClusterConfig c ON a.ClusterId = c.Id
            WHERE {whereClause}
            ORDER BY a.CreatedAt DESC";

        var rows = await conn.QueryAsync<AuditLogDto>(sql, parameters).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<Dictionary<string, int>> GetActionStatisticsAsync(DateTime fromDate, DateTime toDate)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT Action, COUNT(*) AS Count
            FROM AuditLog
            WHERE CreatedAt >= @FromDate AND CreatedAt <= @ToDate
            GROUP BY Action
            ORDER BY COUNT(*) DESC";

        var results = await conn.QueryAsync<(string Action, int Count)>(sql, new { FromDate = fromDate, ToDate = toDate }).ConfigureAwait(false);
        return results.ToDictionary(x => x.Action, x => x.Count);
    }

    private static AuditLog MapToDomain(AuditLogDto dto)
    {
        return new AuditLog
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Action = dto.Action,
            Success = dto.Success,
            CreatedAt = dto.CreatedAt,
            ClusterId = dto.ClusterId,
            ResourceKind = dto.ResourceKind,
            ResourceName = dto.ResourceName,
            Namespace = dto.Namespace,
            ErrorMessage = dto.ErrorMessage,
            RequestPayload = dto.RequestPayload,
            ResponseData = dto.ResponseData,
            IpAddress = dto.IpAddress,
            Duration = dto.Duration,
            User = dto.Username != null ? new AppUser { Id = dto.UserId, Username = dto.Username } : null
        };
    }

    public async Task<IEnumerable<AuditLog>> GetLogsAsync(
        int? userId = null,
        int? clusterId = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        using var conn = _dbFactory.Create();

        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        if (userId.HasValue)
        {
            whereConditions.Add("a.UserId = @UserId");
            parameters.Add("UserId", userId.Value);
        }

        if (clusterId.HasValue)
        {
            whereConditions.Add("a.ClusterId = @ClusterId");
            parameters.Add("ClusterId", clusterId.Value);
        }

        if (!string.IsNullOrEmpty(action))
        {
            whereConditions.Add("a.Action LIKE @Action");
            parameters.Add("Action", $"%{action}%");
        }

        if (fromDate.HasValue)
        {
            whereConditions.Add("a.CreatedAt >= @FromDate");
            parameters.Add("FromDate", fromDate.Value);
        }

        if (toDate.HasValue)
        {
            whereConditions.Add("a.CreatedAt <= @ToDate");
            parameters.Add("ToDate", toDate.Value);
        }

        var whereClause = whereConditions.Any()
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : "";

        var sql = $@"
            SELECT a.*, u.Username, c.Name AS ClusterName
            FROM AuditLog a
            LEFT JOIN AppUser u ON a.UserId = u.Id
            LEFT JOIN ClusterConfig c ON a.ClusterId = c.Id
            {whereClause}
            ORDER BY a.CreatedAt DESC";

        var rows = await conn.QueryAsync<AuditLogDto>(sql, parameters).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    private class AuditLogDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int? ClusterId { get; set; }
        public string Action { get; set; } = null!;
        public string? ResourceKind { get; set; }
        public string? ResourceName { get; set; }
        public string? Namespace { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RequestPayload { get; set; }
        public string? ResponseData { get; set; }
        public string? IpAddress { get; set; }
        public int? Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Username { get; set; }
        public string? ClusterName { get; set; }
    }
}
