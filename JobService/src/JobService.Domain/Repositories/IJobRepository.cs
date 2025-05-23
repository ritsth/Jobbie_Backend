using System.Collections.Generic;
using JobService.Domain.Entities;

namespace JobService.Domain.Repositories
{
    public interface IJobRepository
    {
        Job InsertJob(Job job);
        Job GetById(string jobId);
        IEnumerable<Job> GetAll();
        IEnumerable<Job> GetByStatus(string status);
        Job UpdateJob(Job job);
        void DeleteJob(string jobId);
    }
}
