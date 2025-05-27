using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using JobService.Grpc.Protos;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

namespace AdminService.Clients
{
    public class AdminJobClient:IAdminJobClient
    {
        private readonly JobAdmin.JobAdminClient _client;

        public AdminJobClient(JobAdmin.JobAdminClient client)
        {
            _client = client;
        }

        // Create Job Method
        public async Task CreateJobAsync(NotifyJobRequest request)
        {
            try
            {
                var response = await _client.NotifyJobAsync(request);
                if (response.Success)
                {
                    Console.WriteLine("Job created successfully: " + response.Message);
                }
                else
                {
                    Console.WriteLine("Error: " + response.Message);
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed: {ex.Status.Detail}");
            }
        }

        // Update Job Method
        public async Task UpdateJobAsync(NotifyJobRequest request)
        {

            try
            {
                var response = await _client.NotifyJobAsync(request);
                if (response.Success)
                {
                    Console.WriteLine("Job updated successfully: " + response.Message);
                }
                else
                {
                    Console.WriteLine("Error: " + response.Message);
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed: {ex.Status.Detail}");
            }
        }

        // Delete Job Method
        public async Task DeleteJobAsync(string jobId)
        {
            var request = new NotifyJobRequest
            {
                JobId = jobId,
                Action = "delete"
            };

            try
            {
                var response = await _client.NotifyJobAsync(request);
                if (response.Success)
                {
                    Console.WriteLine("Job deleted successfully: " + response.Message);
                }
                else
                {
                    Console.WriteLine("Error: " + response.Message);
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed: {ex.Status.Detail}");
            }
        }
    }
}
