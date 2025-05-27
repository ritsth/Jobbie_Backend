using JobService.Api.Abstractions;
using JobService.Api.Controllers;
using JobService.Domain.Entities;
using JobService.Domain.Services;
using JobService.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JobService.Api.Tests.Controllers
{
    public class JobControllerTests
    {
        private readonly Mock<IJobService> _jobServiceMock;
        private readonly Mock<IJobAdminClient> _jobAdminClientMock;
        private readonly JobController _controller;

        public JobControllerTests()
        {
            //create mocks for the dependencies
            _jobServiceMock = new Mock<IJobService>();
            _jobAdminClientMock = new Mock<IJobAdminClient>();
            _controller = new JobController(_jobServiceMock.Object, _jobAdminClientMock.Object);
        }

        #region PostJob Tests

        [Fact]
        public async Task JobControllerTests_PostJob_ReturnCreatedAtActionWithJob()
        {
            // Arrange
            var job = new Job
            {
                JobId = "job1",
                Title = "Test Job",
                Description = "Test Description",
                Status = "Pending",
                OwnerId = "owner1",
                CreatedAt = DateTime.UtcNow
            };

            // (Simulate) Define how the mock should respond when specific methods are 
            // called during a test, without relying on the actual implementation
            _jobServiceMock.Setup(s => s.CreateJob(It.IsAny<Job>())).Returns(job);

            _jobAdminClientMock.Setup(c => c.NotifyJobAsync(It.IsAny<NotifyJobRequest>()))
                .ReturnsAsync(new NotifyJobResponse { Success = true });
 
            // Act
            var result = await _controller.PostJob(job);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(job, createdResult.Value);
        }

        [Fact]
        public async Task JobControllerTests_PostJob_ReturnBadRequest()
        {
            // Act
            var result = await _controller.PostJob(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task JobControllerTests_PostJob_ReturnProblemWithFailedGrpc()
        {
            // Arrange
            var job = new Job
            {
                JobId = "job1",
                Title = "Test Job",
                Description = "Test Description",
                Status = "Pending",
                OwnerId = "owner1",
                CreatedAt = DateTime.UtcNow
            };

            _jobServiceMock.Setup(s => s.CreateJob(It.IsAny<Job>())).Returns(job);
            _jobAdminClientMock.Setup(c => c.NotifyJobAsync(It.IsAny<NotifyJobRequest>()))
                .ReturnsAsync(new NotifyJobResponse { Success = false, Message = "gRPC error" });

            // Act
            var result = await _controller.PostJob(job);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        #endregion

        #region GetJobById Tests

        [Fact]
        public void JobControllerTests_GetJobById_ReturnOkWithJob()
        {
            // Arrange
            var jobId = "job1";
            var job = new Job
            {
                JobId = jobId,
                Title = "Test Job",
                Description = "Test Description",
                Status = "Pending",
                OwnerId = "owner1",
                CreatedAt = DateTime.UtcNow
            };

            _jobServiceMock.Setup(s => s.GetJobById(jobId)).Returns(job);

            // Act
            var result = _controller.GetJobById(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedJob = Assert.IsType<Job>(okResult.Value);
            Assert.Equal(jobId, returnedJob.JobId);
        }

        [Fact]
        public void JobControllerTests_GetJobById_ReturnNotFound()
        {
            // Arrange
            var jobId = "nonexistent";
            _jobServiceMock.Setup(s => s.GetJobById(jobId)).Returns((Job)null);

            // Act
            var result = _controller.GetJobById(jobId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region GetAllJobs Tests

        [Fact]
        public void JobControllerTests_GetAllJobs_ReturnOkWithJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { JobId = "job1", Title = "Job 1", Description = "Desc 1", Status = "Pending", OwnerId = "owner1", CreatedAt = DateTime.UtcNow },
                new Job { JobId = "job2", Title = "Job 2", Description = "Desc 2", Status = "Approved", OwnerId = "owner2", CreatedAt = DateTime.UtcNow }
            };

            _jobServiceMock.Setup(s => s.GetAllJobs()).Returns(jobs);

            // Act
            var result = _controller.GetAllJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedJobs = Assert.IsType<List<Job>>(okResult.Value);
        }

        #endregion

        #region GetJobsByStatus Tests

        [Fact]
        public void JobControllerTests_GetJobsByStatus_ReturnOkWithJob()
        {
            // Arrange
            var status = "Pending";
            var jobs = new List<Job>
            {
                new Job { JobId = "job1", Title = "Job 1", Description = "Desc 1", Status = status, OwnerId = "owner1", CreatedAt = DateTime.UtcNow }
            };

            _jobServiceMock.Setup(s => s.GetJobsByStatus(status)).Returns(jobs);

            // Act
            var result = _controller.GetJobsByStatus(status);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedJobs = Assert.IsType<List<Job>>(okResult.Value);
            Assert.Equal(status, returnedJobs[0].Status);
        }

        #endregion

        #region UpdateJob Tests

        [Fact]
        public async Task JobControllerTests_UpdateJob_ReturnOkWithJob()
        {
            // Arrange
            var jobId = "job1";
            var ownerId = "owner1";
            var jobToUpdate = new Job
            {
                JobId = jobId,
                Title = "Updated Job",
                Description = "Updated Description",
                Status = "Approved",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            _jobServiceMock.Setup(s => s.UpdateJob(It.IsAny<Job>(), ownerId)).Returns(jobToUpdate);

            _jobAdminClientMock.Setup(c => c.NotifyJobAsync(It.IsAny<NotifyJobRequest>()))
                .ReturnsAsync(new NotifyJobResponse { Success = true });

            // Act
            var result = await _controller.UpdateJob(jobId, jobToUpdate, ownerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedJob = Assert.IsType<Job>(okResult.Value);
            Assert.Equal(jobId, returnedJob.JobId);
        }

        [Fact]
        public async Task JobControllerTests_UpdateJob_ReturnNotFound()
        {
            // Arrange
            var jobId = "nonexistent";
            var ownerId = "owner1";
            var jobToUpdate = new Job
            {
                JobId = jobId,
                Title = "Updated Job",
                Description = "Updated Description",
                Status = "Approved",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            _jobServiceMock.Setup(s => s.UpdateJob(It.IsAny<Job>(), ownerId)).Returns((Job)null);

            // Act
            var result = await _controller.UpdateJob(jobId, jobToUpdate, ownerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task JobControllerTests_UpdateJob_ReturnProblemWithFailedGrpc()
        {
            // Arrange
            var jobId = "job1";
            var ownerId = "owner1";
            var jobToUpdate = new Job
            {
                JobId = jobId,
                Title = "Updated Job",
                Description = "Updated Description",
                Status = "Approved",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            _jobServiceMock.Setup(s => s.UpdateJob(It.IsAny<Job>(), ownerId)).Returns(jobToUpdate);

            _jobAdminClientMock.Setup(c => c.NotifyJobAsync(It.IsAny<NotifyJobRequest>()))
                .ReturnsAsync(new NotifyJobResponse { Success = false, Message = "gRPC error" });

            // Act
            var result = await _controller.UpdateJob(jobId, jobToUpdate, ownerId);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        #endregion

        #region DeleteJob Tests

        [Fact]
        public async Task JobControllerTests_DeleteJob_ReturnNoContent()
        {
            // Arrange
            var jobId = "job1";
            var ownerId = "owner1";

            _jobServiceMock.Setup(s => s.DeleteJob(jobId, ownerId));

            _jobAdminClientMock.Setup(c => c.NotifyJobAsync(It.IsAny<NotifyJobRequest>()))
                .ReturnsAsync(new NotifyJobResponse { Success = true });

            // Act
            var result = await _controller.DeleteJob(jobId, ownerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task JobControllerTests_DeleteJob_ReturnProblemWithFailedGrpc()
        {
            // Arrange
            var jobId = "job1";
            var ownerId = "owner1";

            _jobServiceMock.Setup(s => s.DeleteJob(jobId, ownerId));
            _jobAdminClientMock.Setup(c => c.NotifyJobAsync(It.IsAny<NotifyJobRequest>()))
                .ReturnsAsync(new NotifyJobResponse { Success = false, Message = "gRPC error" });

            // Act
            var result = await _controller.DeleteJob(jobId, ownerId);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        #endregion
    }
}