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

        public override async Task<NotifyJobResponse> NotifyJob(NotifyJobRequest request, ServerCallContext context)
        {
            string action = request.Action.ToLower();
            bool success = false;
            string message;

            try
            {
                switch (action)
                {
                    case "create":
                        // Handle job creation
                        var newJob = new AdminJobEntity
                        {
                            Title = request.Title,
                            Description = request.Description,
                            Status = request.Status,
                            OwnerId = request.OwnerId,
                            CreatedDateTime = request.CreatedAt.ToDateTime()
                        };
                        var createdJob = _adminJobRepository.InsertJob(newJob);

                        success = true;
                        message = $"Job '{createdJob.Title}' created successfully with ID {createdJob.Id}.";
                        break;

                    case "update":
                        // Handle job update
                        var updateJob = new AdminJobEntity
                        {
                            Id = request.JobId,
                            Title = request.Title,
                            Description = request.Description,
                            Status = request.Status
                        };
                        _adminJobRepository.UpdateJob(updateJob);

                        success = true;
                        message = $"Job '{updateJob.Title}' updated successfully.";
                        break;

                    case "delete":
                        // Handle job deletion
                        _adminJobRepository.DeleteJob(request.JobId);

                        success = true;
                        message = $"Job with ID {request.JobId} deleted successfully.";
                        break;

                    default:
                        message = "Invalid action. Please specify 'Create', 'Update', or 'Delete'.";
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log exception here if necessary
                success = false;
                message = $"Error processing action '{action}': {ex.Message}";
            }

            return await Task.FromResult(new NotifyJobResponse
            {
                Success = success,
                Message = message
            });
        }

    }
}
