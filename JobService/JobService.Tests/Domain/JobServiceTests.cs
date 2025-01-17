using Xunit;
using Moq;
using JobService.Api.Services;
using JobService.Domain.Entities;
using JobService.Domain.Repositories;


namespace JobService.Tests.Domain
{
    public class JobServiceTests
    {
        private readonly Mock<IJobRepository> _mockRepo;
        private readonly JobControlService _jobService;

        public JobServiceTests()
        {
            // Mock the repository
            _mockRepo = new Mock<IJobRepository>();

            // Pass the mock into the real service constructor
            _jobService = new JobControlService(_mockRepo.Object);
        }

        [Fact]
        public void CreateJob_ShouldSetStatusToPending()
        {
            // Arrange
            var newJob = new Job
            {
                Title = "Test Job",
                Description = "This is a test job",
                OwnerId = "owner123"
            };

            // Setup mock to return the same job with an ID
            _mockRepo.Setup(r => r.InsertJob(It.IsAny<Job>()))
                     .Returns((Job j) => { j.Id = 1; return j; });

            // Act
            var created = _jobService.CreateJob(newJob);

            // Assert
            Assert.Equal(1, created.Id);
            Assert.Equal("Pending", created.Status); 
            _mockRepo.Verify(r => r.InsertJob(It.IsAny<Job>()), Times.Once);
        }

        [Fact]
        public void UpdateJob_ShouldThrowIfOwnerDoesNotMatch()
        {
            // Arrange
            var existingJob = new Job
            {
                Id = 1,
                Title = "Old Title",
                OwnerId = "owner123"
            };
            _mockRepo.Setup(r => r.GetById(1)).Returns(existingJob);

            var updatedJob = new Job
            {
                Id = 1,
                Title = "Updated Title",
                OwnerId = "differentOwner"
            };

            // Act & Assert
            Assert.Throws<System.UnauthorizedAccessException>(() =>
            {
                _jobService.UpdateJob(updatedJob, "someone_else");
            });
        }
    }
}
