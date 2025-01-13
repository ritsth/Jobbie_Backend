using Dapper;
using System.Data;
using AdminService.Entities;
using Microsoft.Extensions.Configuration;
using AdminService.Config;

namespace AdminService.Repositories
{
    public class AdminJobRepository : IAdminJobRepository
    {
        private readonly string _connectionString;

        public AdminJobRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AdminJobEntity InsertJob(AdminJobEntity job)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = @"INSERT INTO Jobs (Title, Description, Status, OwnerId, CreatedDateTime)
                           VALUES (@Title, @Description, @Status, @OwnerId, @CreatedDateTime);
                           SELECT LAST_INSERT_ID();";

            // job.CreatedDateTime = job.CreatedDateTime == default ? DateTime.UtcNow : job.CreatedDateTime;
            var id = connection.ExecuteScalar<int>(sql, job);
            job.Id = id;
            job.OwnerId = "Admin";
            return job;
        }

        public AdminJobEntity GetById(int id)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs WHERE Id = @Id";
            return connection.QueryFirstOrDefault<AdminJobEntity>(sql, new { Id = id });
        }

        public IEnumerable<AdminJobEntity> GetAll()
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs";
            return connection.Query<AdminJobEntity>(sql);
        }

        public IEnumerable<AdminJobEntity> GetByStatus(string status)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs WHERE Status = @Status";
            return connection.Query<AdminJobEntity>(sql, new { Status = status });
        }

        public AdminJobEntity UpdateJob(AdminJobEntity job)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = @"UPDATE Jobs 
                           SET Title = @Title, 
                               Description = @Description, 
                               Status = @Status 
                           WHERE Id = @Id";
            connection.Execute(sql, job);
            return job;
        }

        public void DeleteJob(int id)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "DELETE FROM Jobs WHERE Id = @Id";
            connection.Execute(sql, new { Id = id });
        }
    }
}
