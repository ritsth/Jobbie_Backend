using Dapper;
using System.Data;
using AdminService.Infra.Entities;
using Microsoft.Extensions.Configuration;
using AdminService.Infra.Config;

namespace AdminService.Infra.Repositories
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
            string sql = @"INSERT INTO Jobs (JobId, Title, Description, Status, OwnerId, CreatedDateTime)
                           VALUES (@JobId, @Title, @Description, @Status, @OwnerId, @CreatedDateTime);";

            // job.CreatedDateTime = job.CreatedDateTime == default ? DateTime.UtcNow : job.CreatedDateTime;
            // job.Id = Guid.NewGuid().ToString();
            // job.OwnerId = "Admin";
            connection.ExecuteScalar<int>(sql, job);

            return job;
        }

        public AdminJobEntity GetById(string jobId)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs WHERE JobId = @JobId";
            var job = connection.QueryFirstOrDefault<AdminJobEntity>(sql, new { JobId = jobId });
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with JobId {jobId} not found.");
            }
            return job;
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
                           WHERE JobId = @JobId";
            connection.Execute(sql, job);
            return job;
        }

        public void DeleteJob(string jobId)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "DELETE FROM Jobs WHERE JobId = @JobId";
            connection.Execute(sql, new { JobId = jobId });
        }
    }
}
