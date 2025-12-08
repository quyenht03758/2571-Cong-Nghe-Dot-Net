using Dapper;
using K8sManager.Api.Infrastructure;
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    internal class ClusterRepository : IClusterRepository
    {
        private readonly IDbConnectionFactory _factory;
        public ClusterRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> AddAsync(ClusterConfig c)
        {
            const string sql = @"INSERT INTO ClusterConfig(Name, KubeconfigPath, ContextName, AddedBy, CreatedAt)
VALUES(@Name, @KubeconfigPath, @ContextName, @AddedBy, SYSUTCDATETIME());";
            using var db = _factory.Create();
            return await db.ExecuteAsync(sql, c).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ClusterConfig>> GetAllAsync()
        {
            using var db = _factory.Create();
            return await db.QueryAsync<ClusterConfig>("SELECT * FROM ClusterConfig ORDER BY CreatedAt DESC").ConfigureAwait(false);
        }
    }
}
