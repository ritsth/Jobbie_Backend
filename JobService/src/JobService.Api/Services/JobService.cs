using System.Collections.Generic;
using JobService.Domain.Entities;
using JobService.Domain.Repositories;
using JobService.Domain.Services;

namespace JobService.Api.Services
{
    /// <summary>
    /// Implements the domain IJobService interface using the job repository.
    /// </summary>
    public class JobControlService  : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobControlService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public Job CreateJob(Job job)
        {
            job.Status = "Pending"; // default status
            job.JobId = Guid.NewGuid().ToString();
            
            return _jobRepository.InsertJob(job);
        }

        public Job GetJobById(string jobId)
        {
            return _jobRepository.GetById(jobId);
        }

        public IEnumerable<Job> GetAllJobs()
        {
            return _jobRepository.GetAll();
        }

        public IEnumerable<Job> GetJobsByStatus(string status)
        {
            return _jobRepository.GetByStatus(status);
        }

        public Job UpdateJob(Job updatedJob, string ownerId)
        {
            // Check ownership
            var existingJob = _jobRepository.GetById(updatedJob.JobId);
            if (existingJob == null) return null;

            if (existingJob.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("You are not the owner of this job.");

            return _jobRepository.UpdateJob(updatedJob);
        }

        public void DeleteJob(string jobId, string ownerId)
        {
            // Check ownership
            var existingJob = _jobRepository.GetById(jobId);
            if (existingJob == null) return;

            if (existingJob.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("You are not the owner of this job.");

            _jobRepository.DeleteJob(jobId);
        }
    }
}
