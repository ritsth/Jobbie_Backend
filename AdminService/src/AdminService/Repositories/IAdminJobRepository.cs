using AdminService.Entities;

namespace AdminService.Repositories
{
    public interface IAdminJobRepository
    {
        Task<IEnumerable<AdminJobEntity>> GetAllJobsAsync();
        Task<AdminJobEntity> GetJobByIdAsync(int id);
        Task AddJobAsync(AdminJobEntity job);
        Task UpdateJobAsync(AdminJobEntity job);
        Task DeleteJobAsync(int id);
    }
}
