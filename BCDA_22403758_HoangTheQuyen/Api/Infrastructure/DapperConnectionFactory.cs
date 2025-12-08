using System.Data;
using Microsoft.Data.SqlClient;

namespace K8sManager.Api.Infrastructure
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
        string ConnectionString { get; }
    }

    public class DapperConnectionFactory : IDbConnectionFactory
    {
        private readonly string _conn;
        public DapperConnectionFactory(string conn) => _conn = conn;
        public IDbConnection Create() => new SqlConnection(_conn);
        public string ConnectionString => _conn;
    }
}
