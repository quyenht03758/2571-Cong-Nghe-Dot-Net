using Dapper;
using K8sManager.Api.Infrastructure;
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    internal class AuditRepository : IAuditRepository
    {
        private readonly IDbConnectionFactory _factory;
        public AuditRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> AddAsync(AuditLog entry)
        {
            const string sql = @"INSERT INTO AuditLog(UserId, ClusterId, Action, ResourceKind, ResourceName, Namespace, Success, ErrorMessage, RequestPayload, ResponseData, IpAddress, Duration, CreatedAt)
VALUES(@UserId, @ClusterId, @Action, @ResourceKind, @ResourceName, @Namespace, @Success, @ErrorMessage, @RequestPayload, @ResponseData, @IpAddress, @Duration, SYSUTCDATETIME());";
            using var db = _factory.Create();
            return await db.ExecuteAsync(sql, entry).ConfigureAwait(false);
        }

        public Task<int> LogAsync(AuditLog entry) => AddAsync(entry);

        public async Task<IEnumerable<AuditLog>> GetRecentAsync(int days = 7, bool? success = null, int limit = 100)
        {
            using var db = _factory.Create();
            var sql = @"SELECT TOP (@Limit) * FROM AuditLog 
WHERE CreatedAt >= DATEADD(DAY, -@Days, SYSUTCDATETIME())" +
                (success.HasValue ? " AND Success = @Success" : "") + @"
ORDER BY CreatedAt DESC";
            return await db.QueryAsync<AuditLog>(sql, new { Days = days, Success = success, Limit = limit }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId, int limit = 100)
        {
            using var db = _factory.Create();
            var sql = @"SELECT TOP (@Limit) * FROM AuditLog 
WHERE UserId = @UserId 
ORDER BY CreatedAt DESC";
            return await db.QueryAsync<AuditLog>(sql, new { UserId = userId, Limit = limit }).ConfigureAwait(false);
        }
    }
}
