using Dapper;
using K8sManager.Api.Infrastructure;
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    internal class AppUserRepository : IAppUserRepository
    {
        private readonly IDbConnectionFactory _factory;
        public AppUserRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<AppUser?> FindByUsernameAsync(string username)
        {
            using var db = _factory.Create();
            return (await db.QueryAsync<AppUser>("SELECT TOP 1 * FROM AppUser WHERE Username=@u", new { u = username }).ConfigureAwait(false)).FirstOrDefault();
        }

        public Task<AppUser?> GetByUsernameAsync(string username) => FindByUsernameAsync(username);

        public async Task<AppUser?> GetByIdAsync(int id)
        {
            using var db = _factory.Create();
            return (await db.QueryAsync<AppUser>("SELECT TOP 1 * FROM AppUser WHERE Id=@id", new { id }).ConfigureAwait(false)).FirstOrDefault();
        }

        public async Task<int> CreateAsync(AppUser user)
        {
            const string sql = @"INSERT INTO AppUser(Username, PasswordHash, Role, DisplayName, Email, IsLocked, FailedLoginAttempts, CreatedAt, UpdatedAt) 
VALUES(@Username, @PasswordHash, @Role, @DisplayName, @Email, @IsLocked, @FailedLoginAttempts, SYSUTCDATETIME(), SYSUTCDATETIME());";
            using var db = _factory.Create();
            return await db.ExecuteAsync(sql, user).ConfigureAwait(false);
        }

        public async Task<int> UpdateAsync(AppUser user)
        {
            const string sql = @"UPDATE AppUser 
SET PasswordHash=@PasswordHash, Role=@Role, DisplayName=@DisplayName, Email=@Email, 
    IsLocked=@IsLocked, FailedLoginAttempts=@FailedLoginAttempts, 
    LastLoginAt=@LastLoginAt, LastPasswordChangedAt=@LastPasswordChangedAt, 
    UpdatedAt=SYSUTCDATETIME()
WHERE Id=@Id";
            using var db = _factory.Create();
            return await db.ExecuteAsync(sql, user).ConfigureAwait(false);
        }
    }
}
