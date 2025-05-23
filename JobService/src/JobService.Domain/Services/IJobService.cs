using System.Collections.Generic;
using JobService.Domain.Entities;

namespace JobService.Domain.Services
{
    public interface IJobService
    {
        Job CreateJob(Job job);
        Job GetJobById(string jobId);
        IEnumerable<Job> GetAllJobs();
        IEnumerable<Job> GetJobsByStatus(string status);
        Job UpdateJob(Job updatedJob, string ownerId);
        void DeleteJob(string jobId, string ownerId);
    }
}
