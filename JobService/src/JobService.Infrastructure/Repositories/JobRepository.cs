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
            string sql = @"INSERT INTO Jobs (Title, Description, Status, OwnerId, CreatedAt)
                           VALUES (@Title, @Description, @Status, @OwnerId, @CreatedAt);
                           SELECT LAST_INSERT_ID();";

            job.CreatedAt = job.CreatedAt == default ? DateTime.UtcNow : job.CreatedAt;
            var id = connection.ExecuteScalar<int>(sql, job);
            job.Id = id;
            return job;
        }

        public Job GetById(int id)
        {
            using var connection = MySqlDapperConfig.CreateConnection(_connectionString);
            string sql = "SELECT * FROM Jobs WHERE Id = @Id";
            return connection.QueryFirstOrDefault<Job>(sql, new { Id = id });
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
