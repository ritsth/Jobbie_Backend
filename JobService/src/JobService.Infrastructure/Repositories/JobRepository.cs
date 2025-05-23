using System.Collections.Generic;
using System.Linq;
using Dapper;
using JobService.Domain.Entities;
using JobService.Domain.Repositories;
using JobService.Infrastructure.Config;

namespace JobService.Infrastructure.Repositories
{
    /// <summary>
    /// Dapper-based repository that handles Job CRUD operations.
    /// </summary>
    public class JobRepository : IJobRepository
    {
        private readonly string _connectionString;

        public JobRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Job InsertJob(Job job)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = @"INSERT INTO Jobs (JobId, Title, Description, Status, OwnerId, CreatedAt)
                           VALUES (@JobId, @Title, @Description, @Status, @OwnerId, @CreatedAt);";

            job.CreatedAt = job.CreatedAt == default ? DateTime.UtcNow : job.CreatedAt;
            //job.JobId = Guid.NewGuid().ToString();

            connection.ExecuteScalar<int>(sql, job);

            return job;
        }

        public Job GetById(string  jobId)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs WHERE JobId = @JobId";
            return connection.QueryFirstOrDefault<Job>(sql, new { JobId = jobId });
        }

        public IEnumerable<Job> GetAll()
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs";
            return connection.Query<Job>(sql);
        }

        public IEnumerable<Job> GetByStatus(string status)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs WHERE Status = @Status";
            return connection.Query<Job>(sql, new { Status = status });
        }

        public Job UpdateJob(Job job)
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

        public void DeleteJob(string  jobId)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "DELETE FROM Jobs WHERE Id = @JobId";
            connection.Execute(sql, new { JobId = jobId });
        }
    }
}
