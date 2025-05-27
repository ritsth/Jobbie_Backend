using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AdminGrpcService.Services;
using AdminService.Infra.Repositories;
using AdminService.Infra.Entities;
using JobService.Grpc.Protos;
using System.Threading.Tasks;
using System;

namespace AdminGrpcService.Tests
{
    public class AdminJobServiceTests
    {
        private readonly Mock<IAdminJobRepository> _mockRepo;
        private readonly Mock<ILogger<AdminJobService>> _mockLogger;
        private readonly AdminJobService _service;

        public AdminJobServiceTests()
        {
            _mockRepo = new Mock<IAdminJobRepository>();
            _mockLogger = new Mock<ILogger<AdminJobService>>();
            _service = new AdminJobService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task NotifyJob_Create_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new NotifyJobRequest
            {
                Action = "create",
                JobId = Guid.NewGuid().ToString(),
                Title = "Test Job",
                Description = "Test Description",
                Status = "Pending",
                OwnerId = "test-user",
                CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
            };

            _mockRepo.Setup(r => r.InsertJob(It.IsAny<AdminJobEntity>())).Returns((AdminJobEntity e) => e);

            // Act
            var result = await _service.NotifyJob(request, null);

            // Assert
            Assert.True(result.Success);
            Assert.Contains("created successfully", result.Message);
        }

        [Fact]
        public async Task NotifyJob_Update_JobNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var request = new NotifyJobRequest
            {
                Action = "update",
                JobId = "non-existent",
                Title = "Updated Job",
                Description = "Updated Description",
                Status = "Approved"
            };

            _mockRepo.Setup(r => r.GetById(It.IsAny<string>())).Returns((AdminJobEntity)null);

            // Act
            var result = await _service.NotifyJob(request, null);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("not found", result.Message);
        }

        [Fact]
        public async Task NotifyJob_Delete_ReturnsSuccessResponse()
        {
            // Arrange
            var jobId = Guid.NewGuid().ToString();
            var request = new NotifyJobRequest
            {
                Action = "delete",
                JobId = jobId,
                Title = "ToDelete Job"
            };

            _mockRepo.Setup(r => r.GetById(jobId)).Returns(new AdminJobEntity { JobId = jobId });
            _mockRepo.Setup(r => r.DeleteJob(jobId));

            // Act
            var result = await _service.NotifyJob(request, null);

            // Assert
            Assert.True(result.Success);
            Assert.Contains("deleted successfully", result.Message);
        }

        [Fact]
        public async Task NotifyJob_InvalidAction_ReturnsFailureResponse()
        {
            // Arrange
            var request = new NotifyJobRequest { Action = "invalid" };

            // Act
            var result = await _service.NotifyJob(request, null);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid action", result.Message);
        }
    }
}
