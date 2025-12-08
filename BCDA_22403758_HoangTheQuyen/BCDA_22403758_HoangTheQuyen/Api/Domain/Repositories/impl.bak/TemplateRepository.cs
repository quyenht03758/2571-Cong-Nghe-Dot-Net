using Dapper;
using K8sManager.Api.Infrastructure;
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    internal class TemplateRepository : ITemplateRepository
    {
        private readonly IDbConnectionFactory _factory;
        public TemplateRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> AddAsync(Template t)
        {
            const string sql = @"INSERT INTO Template(Name, Description, YamlText, CreatedBy, CreatedAt)
VALUES(@Name, @Description, @YamlText, @CreatedBy, SYSUTCDATETIME());";
            using var db = _factory.Create();
            return await db.ExecuteAsync(sql, t).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Template>> GetAllAsync()
        {
            using var db = _factory.Create();
            return await db.QueryAsync<Template>("SELECT * FROM Template ORDER BY CreatedAt DESC").ConfigureAwait(false);
        }
    }
}
