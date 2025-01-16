using Xunit;
using Moq;
using System.Data;
using Dapper;
using JobService.Infrastructure.Repositories;
using JobService.Domain.Entities;

namespace JobService.Tests.Infrastructure
{
    public class JobRepositoryTests
    {
        [Fact]
        public void InsertJob_ShouldInsertIntoRealMySql()
        {
            // 1. Start Testcontainers MySQL (or ensure local MySQL is running)
            // 2. Create a valid connection string
            var validConnString = "Server=127.0.0.1;Port=3306;Database=testdb;User=root;Password=testpw;";

            // 3. Possibly run an "Initialize DB" script here

            var repo = new JobRepository(validConnString);

            var newJob = new Job { Title = "Real Insert", OwnerId = "owner" };

            // Act
            var result = repo.InsertJob(newJob);

            // Assert - you can query back or check row count
            Assert.True(result.Id > 0);
        }
    }
}
