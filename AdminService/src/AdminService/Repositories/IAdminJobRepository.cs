using AdminService.Entities;

namespace AdminService.Repositories
{
    public interface IAdminJobRepository
    {
        AdminJobEntity InsertJob (AdminJobEntity AdminJobEntity);
        AdminJobEntity GetById(int id);
        IEnumerable<AdminJobEntity> GetAll();
        IEnumerable<AdminJobEntity> GetByStatus(string status);
        AdminJobEntity UpdateJob(AdminJobEntity AdminJobEntity);
        void DeleteJob(int id);
    }
}
