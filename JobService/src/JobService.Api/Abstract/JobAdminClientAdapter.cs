using JobService.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;
using JobService.Api.Abstractions;

namespace JobService.Api.Adapters
{
    public class JobAdminClientAdapter : IJobAdminClient
    {
        private readonly JobAdmin.JobAdminClient _jobAdminClient;

        public JobAdminClientAdapter(JobAdmin.JobAdminClient jobAdminClient)
        {
            _jobAdminClient = jobAdminClient ?? throw new ArgumentNullException(nameof(jobAdminClient));
        }

        public async Task<NotifyJobResponse> NotifyJobAsync(NotifyJobRequest request)
        {
            return await _jobAdminClient.NotifyJobAsync(request);
        }
    }
}