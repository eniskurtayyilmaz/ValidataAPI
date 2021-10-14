using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ValidataAPI.Utils.Common;

namespace ValidataAPI.Utils.Repositories
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }

    public class SqlConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var sqlConnection = new SqlConnection(_configuration.GetConnectionString(Constants.DatabaseConnectionStringKey));
            await sqlConnection.OpenAsync();
            return sqlConnection;
        }
    }
}