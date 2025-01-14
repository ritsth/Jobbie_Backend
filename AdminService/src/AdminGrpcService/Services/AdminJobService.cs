using System;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using JobService.Grpc.Protos;
using AdminService.Repositories;
using AdminService.Entities;

namespace AdminGrpcService.Services
{
    public class AdminJobService : JobAdmin.JobAdminBase
    {
        private readonly IAdminJobRepository _adminJobRepository;

        public AdminJobService(IAdminJobRepository adminJobRepository)
        {
            _adminJobRepository = adminJobRepository;
        }
        public override Task<NotifyJobResponse> NotifyJob(NotifyJobRequest request, ServerCallContext context)
        {
            // Simulate processing the job notification based on the action
            string action = request.Action.ToLower();
            bool success = false;
            string message;

            switch (action)
            {
                case "create":
                    // Handle job creation logic in database
                    // !!!
                    _adminJobRepository.InsertJob(new AdminJobEntity
                    {
                        // Title = request.Title,
                        // Description = request.Description,
                        // Status = request.Status,
                        // CreatedDateTime  = DateTime.UtcNow
                    });


                    success = true; // Assume successful creation
                    message = $"Job '{request.Title}' created successfully.";
                    break;
                case "update":
                    // Handle job update logic
                    success = true; // Assume successful update
                    message = $"Job '{request.Title}' updated successfully.";
                    break;
                case "delete":
                    // Handle job deletion logic
                    success = true; // Assume successful deletion
                    message = $"Job '{request.Title}' deleted successfully.";
                    break;
                default:
                    message = "Invalid action. Please specify 'Create', 'Update', or 'Delete'.";
                    break;
            }

            return Task.FromResult(new NotifyJobResponse
            {
                Success = success,
                Message = message
            });
        }
    }
}
