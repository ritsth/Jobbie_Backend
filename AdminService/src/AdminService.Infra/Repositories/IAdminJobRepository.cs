using AdminService.Infra.Entities;

namespace AdminService.Infra.Repositories
{
    public interface IAdminJobRepository
    {
        AdminJobEntity InsertJob (AdminJobEntity AdminJobEntity);
        AdminJobEntity GetById(string jobId);
        IEnumerable<AdminJobEntity> GetAll();
        IEnumerable<AdminJobEntity> GetByStatus(string status);
        AdminJobEntity UpdateJob(AdminJobEntity AdminJobEntity);
        void DeleteJob(string jobId);
    }
}
