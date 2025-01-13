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
            return _jobRepository.InsertJob(job);
        }

        public Job GetJobById(int id)
        {
            return _jobRepository.GetById(id);
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
            var existingJob = _jobRepository.GetById(updatedJob.Id);
            if (existingJob == null) return null;

            if (existingJob.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("You are not the owner of this job.");

            return _jobRepository.UpdateJob(updatedJob);
        }

        public void DeleteJob(int id, string ownerId)
        {
            // Check ownership
            var existingJob = _jobRepository.GetById(id);
            if (existingJob == null) return;

            if (existingJob.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("You are not the owner of this job.");

            _jobRepository.DeleteJob(id);
        }
    }
}
