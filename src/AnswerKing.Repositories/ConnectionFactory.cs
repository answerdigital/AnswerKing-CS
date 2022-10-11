using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using AnswerKing.Repositories.Interfaces;

namespace AnswerKing.Repositories
{
    public class ConnectionFactory : IConnectionFactory
    {
        private IConfiguration _configuration;

        public ConnectionFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = this._configuration.GetConnectionString("SqlDatabase");
            return new SqlConnection(connectionString);
        }
    }
}