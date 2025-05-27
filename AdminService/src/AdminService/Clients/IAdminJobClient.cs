using JobService.Grpc.Protos;

namespace AdminService.Clients
{
    public interface IAdminJobClient
    {
        Task CreateJobAsync(NotifyJobRequest request);
        Task UpdateJobAsync(NotifyJobRequest request);
        Task DeleteJobAsync(string jobId);
    }
}