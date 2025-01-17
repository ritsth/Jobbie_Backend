using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using JobService.Domain.Services;
using JobService.Domain.Entities;
using JobService.Api.Controllers;
using System.Collections.Generic;

namespace JobService.Tests.Api
{
    public class JobControllerTests
    {
        private readonly JobController _controller;
        private readonly Mock<IJobService> _mockService;

        public JobControllerTests()
        {
            _mockService = new Mock<IJobService>();
            _controller = new JobController(_mockService.Object);
        }

        [Fact]
        public void GetAllJobs_ShouldReturnOkWithJobs()
        {
            // Arrange
            var mockJobs = new List<Job>()
            {
                new Job { Id = 1, Title = "Test1" },
                new Job { Id = 2, Title = "Test2" }
            };
            _mockService.Setup(s => s.GetAllJobs()).Returns(mockJobs);

            // Act
            var result = _controller.GetAllJobs() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedJobs = Assert.IsType<List<Job>>(result.Value);
            Assert.Equal(2, returnedJobs.Count);
        }

        [Fact]
        public void GetJobById_JobNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetJobById(99)).Returns((Job)null);

            // Act
            var result = _controller.GetJobById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void PostJob_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var newJob = new Job { Title = "ControllerTest", OwnerId = "owner123" };
            var createdJob = new Job { Id = 10, Title = "ControllerTest", OwnerId = "owner123", Status = "Pending" };

            _mockService.Setup(s => s.CreateJob(It.IsAny<Job>())).Returns(createdJob);

            // Act
            var actionResult = _controller.PostJob(newJob) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.Equal("GetJobById", actionResult.ActionName);
            Assert.Equal(10, actionResult.RouteValues["id"]);
            var jobValue = actionResult.Value as Job;
            Assert.Equal("ControllerTest", jobValue.Title);
            Assert.Equal("Pending", jobValue.Status);
        }
    }
}
