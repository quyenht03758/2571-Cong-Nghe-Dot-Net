// Api/Infrastructure/Repositories/UserRepositoryAdapter.cs
using Dapper;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Domain.ValueObjects;
using K8sManager.Api.Infrastructure;

namespace K8sManager.Api.Infrastructure.Repositories;

/// <summary>
/// Adapter from existing Dapper implementation to Domain repository interface
/// Maps between database models and domain entities
/// </summary>
public class UserRepositoryAdapter : IUserRepository
{
    private readonly DapperConnectionFactory _dbFactory;

    public UserRepositoryAdapter(DapperConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var conn = _dbFactory.Create();
        var row = await conn.QueryFirstOrDefaultAsync<UserDto>(
            "SELECT * FROM AppUser WHERE Id = @Id",
            new { Id = id }).ConfigureAwait(false);

        return row == null ? null : MapToDomain(row);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var conn = _dbFactory.Create();
        var row = await conn.QueryFirstOrDefaultAsync<UserDto>(
            "SELECT * FROM AppUser WHERE Username = @Username",
            new { Username = username }).ConfigureAwait(false);

        return row == null ? null : MapToDomain(row);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var conn = _dbFactory.Create();
        var rows = await conn.QueryAsync<UserDto>("SELECT * FROM AppUser ORDER BY CreatedAt DESC").ConfigureAwait(false);
        return rows.Select(MapToDomain);
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null)
    {
        using var conn = _dbFactory.Create();

        var offset = (pageNumber - 1) * pageSize;
        var whereClause = string.IsNullOrWhiteSpace(searchTerm)
            ? ""
            : "WHERE Username LIKE @Search OR Email LIKE @Search OR DisplayName LIKE @Search";

        var sql = $@"
            SELECT * FROM AppUser
            {whereClause}
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*) FROM AppUser {whereClause};";

        var searchParam = $"%{searchTerm}%";
        using var multi = await conn.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize, Search = searchParam }).ConfigureAwait(false);

        var items = (await multi.ReadAsync<UserDto>().ConfigureAwait(false)).Select(MapToDomain);
        var totalCount = await multi.ReadSingleAsync<int>().ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task<int> CreateAsync(User user)
    {
        using var conn = _dbFactory.Create();

        var sql = @"
            INSERT INTO AppUser (Username, PasswordHash, Role, Email, DisplayName, IsLocked,
                                 FailedLoginAttempts, LastPasswordChangedAt, CreatedAt, UpdatedAt)
            VALUES (@Username, @PasswordHash, @Role, @Email, @DisplayName, @IsLocked,
                    @FailedLoginAttempts, @LastPasswordChangedAt, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            user.Username,
            user.PasswordHash,
            Role = user.Role.ToString(),
            Email = user.Email?.Value,
            user.DisplayName,
            user.IsLocked,
            user.FailedLoginAttempts,
            user.LastPasswordChangedAt,
            user.CreatedAt,
            user.UpdatedAt
        }).ConfigureAwait(false);

        return id;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        using var conn = _dbFactory.Create();

        var sql = @"
            UPDATE AppUser
            SET Username = @Username,
                PasswordHash = @PasswordHash,
                Role = @Role,
                Email = @Email,
                DisplayName = @DisplayName,
                IsLocked = @IsLocked,
                FailedLoginAttempts = @FailedLoginAttempts,
                LastLoginAt = @LastLoginAt,
                LastPasswordChangedAt = @LastPasswordChangedAt,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        var rowsAffected = await conn.ExecuteAsync(sql, new
        {
            user.Id,
            user.Username,
            user.PasswordHash,
            Role = user.Role.ToString(),
            Email = user.Email?.Value,
            user.DisplayName,
            user.IsLocked,
            user.FailedLoginAttempts,
            user.LastLoginAt,
            user.LastPasswordChangedAt,
            user.UpdatedAt
        }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = _dbFactory.Create();
        var rowsAffected = await conn.ExecuteAsync("DELETE FROM AppUser WHERE Id = @Id", new { Id = id }).ConfigureAwait(false);
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(string username)
    {
        using var conn = _dbFactory.Create();
        var count = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM AppUser WHERE Username = @Username",
            new { Username = username }).ConfigureAwait(false);
        return count > 0;
    }

    // Helper method to map DB row to domain entity
    private User MapToDomain(UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = string.IsNullOrEmpty(dto.Email) ? null : Email.Create(dto.Email),
            DisplayName = dto.DisplayName,
            PasswordHash = dto.PasswordHash,
            Role = Enum.Parse<UserRole>(dto.Role),
            IsLocked = dto.IsLocked,
            FailedLoginAttempts = dto.FailedLoginAttempts,
            LastLoginAt = dto.LastLoginAt,
            LastPasswordChangedAt = dto.LastPasswordChangedAt,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    // DTO for Dapper mapping
    private class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastPasswordChangedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
