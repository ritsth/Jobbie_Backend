using System.Collections.Generic;
using JobService.Domain.Entities;

namespace JobService.Domain.Repositories
{
    public interface IJobRepository
    {
        Job InsertJob(Job job);
        Job GetById(int id);
        IEnumerable<Job> GetAll();
        IEnumerable<Job> GetByStatus(string status);
        Job UpdateJob(Job job);
        void DeleteJob(int id);
    }
}
