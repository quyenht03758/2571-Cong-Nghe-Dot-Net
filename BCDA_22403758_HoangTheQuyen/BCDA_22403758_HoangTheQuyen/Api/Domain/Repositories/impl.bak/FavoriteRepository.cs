using Dapper;
using K8sManager.Api.Infrastructure;
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    internal class FavoriteRepository : IFavoriteRepository
    {
        private readonly IDbConnectionFactory _factory;
        public FavoriteRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> AddAsync(Favorite f)
        {
            const string sql = @"INSERT INTO Favorite(UserId, ClusterId, Namespace, Kind, Query, DisplayName, CreatedAt)
VALUES(@UserId, @ClusterId, @Namespace, @Kind, @Query, @DisplayName, SYSUTCDATETIME());";
            using var db = _factory.Create();
            return await db.ExecuteAsync(sql, f).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Favorite>> ListByUserAsync(int userId)
        {
            using var db = _factory.Create();
            return await db.QueryAsync<Favorite>("SELECT * FROM Favorite WHERE UserId=@u ORDER BY CreatedAt DESC", new { u = userId }).ConfigureAwait(false);
        }
    }
}
