using JobService.Grpc.Protos;

namespace JobService.Api.Abstractions
{
    public interface IJobAdminClient
    {
        Task<NotifyJobResponse> NotifyJobAsync(NotifyJobRequest request);
    }
}