using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AdminService.Controllers;
using AdminService.Infra.Repositories;
using AdminService.Clients;
using AdminService.Infra.Entities;
using JobService.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApiService.Tests
{
    public class AdminJobControllerTests
    {
        private readonly Mock<IAdminJobRepository> _mockRepo;
        private readonly Mock<IAdminJobClient> _mockClient;
        private readonly AdminJobController _controller;

        public AdminJobControllerTests()
        {
            _mockRepo = new Mock<IAdminJobRepository>();
            _mockClient = new Mock<IAdminJobClient>();
            _controller = new AdminJobController(_mockRepo.Object, _mockClient.Object);
        }

        [Fact]
        public void GetAllJobs_ReturnsOkResultWithJobs()
        {
            var jobs = new List<AdminJobEntity> {
                new AdminJobEntity { JobId = "1", Title = "Job 1", Description = "Desc", OwnerId = "admin", CreatedDateTime = DateTime.UtcNow }
            };
            _mockRepo.Setup(repo => repo.GetAll()).Returns(jobs);

            var result = _controller.GetAllJobs();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(jobs, okResult.Value);
        }

        [Fact]
        public void GetJobById_JobFound_ReturnsOk()
        {
            var jobId = "abc123";
            var job = new AdminJobEntity { JobId = jobId, Title = "Test Job", OwnerId = "admin" };
            _mockRepo.Setup(r => r.GetById(jobId)).Returns(job);

            var result = _controller.GetJobById(jobId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(job, okResult.Value);
        }

        [Fact]
        public void GetJobById_JobNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetById("notfound")).Returns((AdminJobEntity)null!);

            var result = _controller.GetJobById("notfound");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateJob_ValidJob_ReturnsCreatedAt()
        {
            var job = new AdminJobEntity
            {
                JobId = Guid.NewGuid().ToString(),
                Title = "New Job",
                Description = "Desc",
                Status = "Pending",
                OwnerId = "admin",
                CreatedDateTime = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.InsertJob(job)).Returns(job);
            _mockClient.Setup(c => c.CreateJobAsync(It.IsAny<NotifyJobRequest>())).Returns(Task.CompletedTask);

            var result = _controller.CreateJob(job);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(job, createdAtResult.Value);
        }

        [Fact]
        public void UpdateJob_Valid_ReturnsOk()
        {
            var job = new AdminJobEntity
            {
                JobId = "xyz",
                Title = "Updated Job",
                Description = "Updated Desc",
                Status = "Approved",
                OwnerId = "admin",
                CreatedDateTime = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.UpdateJob(job)).Returns(job);
            _mockClient.Setup(c => c.UpdateJobAsync(It.IsAny<NotifyJobRequest>())).Returns(Task.CompletedTask);

            var result = _controller.UpdateJob("xyz", job);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(job, okResult.Value);
        }

        [Fact]
        public void DeleteJob_ValidId_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteJob("abc")).Verifiable();
            _mockClient.Setup(c => c.DeleteJobAsync("abc")).Returns(Task.CompletedTask);

            var result = _controller.DeleteJob("abc");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetJobsByStatus_WithResults_ReturnsOk()
        {
            var jobs = new List<AdminJobEntity> { new() { JobId = "1", Status = "Pending", Title = "test", OwnerId = "admin", CreatedDateTime = DateTime.UtcNow } };
            _mockRepo.Setup(r => r.GetByStatus("Pending")).Returns(jobs);

            var result = _controller.GetJobsByStatus("Pending");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(jobs, okResult.Value);
        }

        [Fact]
        public void GetJobsByStatus_EmptyList_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByStatus("Done")).Returns(new List<AdminJobEntity>());

            var result = _controller.GetJobsByStatus("Done");

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
