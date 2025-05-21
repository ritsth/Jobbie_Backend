using System.Collections.Generic;
using JobService.Domain.Entities;

namespace JobService.Domain.Services
{
    public interface IJobService
    {
        Job CreateJob(Job job);
        Job GetJobById(int id);
        IEnumerable<Job> GetAllJobs();
        IEnumerable<Job> GetJobsByStatus(string status);
        Job UpdateJob(Job updatedJob, string ownerId);
        void DeleteJob(int id, string ownerId);
    }
}
