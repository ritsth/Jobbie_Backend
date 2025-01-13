using System.Data;
using MySql.Data.MySqlClient;

namespace AdminService.Config
{
    /// <summary>
    /// Provides a way to create and manage MySQL connections using Dapper.
    /// </summary>
    public static class MySqlDapperConfig
    {
        public static IDbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}