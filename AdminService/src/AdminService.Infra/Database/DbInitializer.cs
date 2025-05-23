using System.Data;
using Dapper;

namespace AdminService.Infra.Database
{
    public static class DbInitializer
    {
        public static void Initialize(IDbConnection connection)
        {
            var tableCheck = @"CREATE TABLE IF NOT EXISTS Jobs (
                                JobId CHAR(36) PRIMARY NOT NULL KEY DEFAULT UUID(),
                                Title VARCHAR(255) NOT NULL,
                                Description TEXT NOT NULL,
                                Status VARCHAR(50) NOT NULL,
                                OwnerId VARCHAR(100) NOT NULL,
                                CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                               );";
            connection.Execute(tableCheck);
        }
    }
}