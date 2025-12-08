using Dapper;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Infrastructure;

namespace K8sManager.Api.Infrastructure.Repositories;

public class UserSessionRepositoryAdapter : IUserSessionRepository
{
    private readonly DapperConnectionFactory _dbFactory;

    public UserSessionRepositoryAdapter(DapperConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<UserSession?> GetByIdAsync(long id)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT s.*, u.Username
            FROM UserSession s
            LEFT JOIN AppUser u ON s.UserId = u.Id
            WHERE s.Id = @Id";

        var dto = await conn.QueryFirstOrDefaultAsync<UserSessionDto>(sql, new { Id = id }).ConfigureAwait(false);
        return dto == null ? null : MapToDomain(dto);
    }

    public async Task<UserSession?> GetByTokenAsync(string sessionToken)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT s.*, u.Username
            FROM UserSession s
            LEFT JOIN AppUser u ON s.UserId = u.Id
            WHERE s.SessionToken = @SessionToken";

        var dto = await conn.QueryFirstOrDefaultAsync<UserSessionDto>(sql, new { SessionToken = sessionToken }).ConfigureAwait(false);
        return dto == null ? null : MapToDomain(dto);
    }

    public async Task<IEnumerable<UserSession>> GetByUserIdAsync(int userId)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            SELECT s.*, u.Username
            FROM UserSession s
            LEFT JOIN AppUser u ON s.UserId = u.Id
            WHERE s.UserId = @UserId
            ORDER BY s.CreatedAt DESC";

        var rows = await conn.QueryAsync<UserSessionDto>(sql, new { UserId = userId }).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync(int? userId = null)
    {
        using var conn = _dbFactory.Create();
        var sql = userId.HasValue
            ? @"SELECT s.*, u.Username
                FROM UserSession s
                LEFT JOIN AppUser u ON s.UserId = u.Id
                WHERE s.UserId = @UserId AND s.ExpiresAt > SYSUTCDATETIME()
                ORDER BY s.CreatedAt DESC"
            : @"SELECT s.*, u.Username
                FROM UserSession s
                LEFT JOIN AppUser u ON s.UserId = u.Id
                WHERE s.ExpiresAt > SYSUTCDATETIME()
                ORDER BY s.CreatedAt DESC";

        var rows = await conn.QueryAsync<UserSessionDto>(sql, userId.HasValue ? new { UserId = userId.Value } : null).ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<long> CreateAsync(UserSession session)
    {
        using var conn = _dbFactory.Create();
        var sql = @"
            INSERT INTO UserSession (UserId, SessionToken, IpAddress, UserAgent, ExpiresAt, CreatedAt)
            VALUES (@UserId, @SessionToken, @IpAddress, @UserAgent, @ExpiresAt, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS bigint);";

        var id = await conn.ExecuteScalarAsync<long>(sql, new
        {
            session.UserId,
            session.SessionToken,
            session.IpAddress,
            session.UserAgent,
            session.ExpiresAt,
            session.CreatedAt
        }).ConfigureAwait(false);

        return id;
    }

    public async Task<int> DeleteAsync(long id)
    {
        using var conn = _dbFactory.Create();
        var rowsAffected = await conn.ExecuteAsync("DELETE FROM UserSession WHERE Id = @Id", new { Id = id }).ConfigureAwait(false);
        return rowsAffected;
    }

    public async Task<bool> DeleteByTokenAsync(string sessionToken)
    {
        using var conn = _dbFactory.Create();
        var rowsAffected = await conn.ExecuteAsync("DELETE FROM UserSession WHERE SessionToken = @SessionToken", new { SessionToken = sessionToken }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<int> DeleteByUserIdAsync(int userId)
    {
        using var conn = _dbFactory.Create();
        var rowsAffected = await conn.ExecuteAsync("DELETE FROM UserSession WHERE UserId = @UserId", new { UserId = userId }).ConfigureAwait(false);
        return rowsAffected;
    }

    public async Task DeleteExpiredAsync()
    {
        using var conn = _dbFactory.Create();
        await conn.ExecuteAsync("DELETE FROM UserSession WHERE ExpiresAt <= SYSUTCDATETIME()").ConfigureAwait(false);
    }

    private static UserSession MapToDomain(UserSessionDto dto)
    {
        return new UserSession
        {
            Id = dto.Id,
            UserId = dto.UserId,
            SessionToken = dto.SessionToken,
            ExpiresAt = dto.ExpiresAt,
            CreatedAt = dto.CreatedAt,
            IpAddress = dto.IpAddress,
            UserAgent = dto.UserAgent
        };
    }

    private class UserSessionDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string SessionToken { get; set; } = null!;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Username { get; set; }
    }
}
