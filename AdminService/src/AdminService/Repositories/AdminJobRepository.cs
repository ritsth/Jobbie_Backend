using Dapper;
using System.Data;
using AdminService.Entities;
using Microsoft.Extensions.Configuration;

namespace AdminService.Repositories
{
    public class AdminJobRepository : IAdminJobRepository
    {
        private readonly IDbConnection _dbConnection;

        public AdminJobRepository(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbConnection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
        }

        public async Task<IEnumerable<AdminJobEntity>> GetAllJobsAsync()
        {
            // Console.log("GetAllJobsAsync");
            var sql = "SELECT * FROM AdminJobEntities WHERE Status != 'Deleted'";
            return await _dbConnection.QueryAsync<AdminJobEntity>(sql);
        }

        public async Task<AdminJobEntity> GetJobByIdAsync(int id)
        {
            var sql = "SELECT * FROM AdminJobEntities WHERE Id = @Id AND Status != 'Deleted'";
            return await _dbConnection.QueryFirstOrDefaultAsync<AdminJobEntity>(sql, new { Id = id });
        }

        public async Task AddJobAsync(AdminJobEntity job)
        {
            var sql = @"INSERT INTO AdminJobEntities (Title, Description, Status, OwnerId, CreatedDateTime) 
                        VALUES (@Title, @Description, @Status, @OwnerId, @CreatedDateTime)";
            await _dbConnection.ExecuteAsync(sql, job);
        }

        public async Task UpdateJobAsync(AdminJobEntity job)
        {
            var sql = @"UPDATE AdminJobEntities 
                        SET Title = @Title, Description = @Description, Status = @Status, OwnerId = @OwnerId 
                        WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, job);
        }

        public async Task DeleteJobAsync(int id)
        {
            var sql = "UPDATE AdminJobEntities SET Status = 'Deleted' WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
