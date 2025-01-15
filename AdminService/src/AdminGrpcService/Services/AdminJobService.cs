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

        private async Task<NotifyJobResponse> HandleCreateJob(NotifyJobRequest request)
        {
            //notify the user about the approval request (using notification service? Rest api?)
            //then if yes, create the job, and success: true
            //if no, success: false

            bool success = false;
            string message;

            // Simulate user approval for job creation
            bool userResponse = true; // This should be replaced with actual user response logic

            if (!userResponse)
            {
                success = false;
                message = $"Admin declined the request to create '{request.Title}' job.";
                return new NotifyJobResponse { Success = success, Message = message };
            }

            try
            {
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
                message = $"Approved! Job '{createdJob.Title}' created successfully with ID {createdJob.Id}.";
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error processing job creation: {ex.Message}";
            }

            return new NotifyJobResponse { Success = success, Message = message };
        }

        private async Task<NotifyJobResponse> HandleUpdateJob(NotifyJobRequest request)
        {
            bool success = false;
            string message;

            // Simulate user approval for job creation
            bool userResponse = true; // This should be replaced with actual user response logic

            if (!userResponse)
            {
                success = false;
                message = $"Admin declined the request to update '{request.Title}' job.";
                return new NotifyJobResponse { Success = success, Message = message };
            }

            try
            {
                var job = _adminJobRepository.GetById(request.JobId);
                if (job == null)
                {
                    success = false;
                    message = $"Job with ID {request.JobId} not found.";
                    return new NotifyJobResponse { Success = success, Message = message };
                }

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
                message = $"Approved! Job '{updateJob.Title}' updated successfully.";
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error processing job update: {ex.Message}";
            }

            return new NotifyJobResponse { Success = success, Message = message };
        }

        private async Task<NotifyJobResponse> HandleDeleteJob(NotifyJobRequest request)
        {
            bool success = false;
            string message;

            // Simulate user approval for job creation
            bool userResponse = true; // This should be replaced with actual user response logic

            if (!userResponse)
            {
                success = false;
                message = $"Admin declined the request to delete '{request.Title}' the job.";
                return new NotifyJobResponse { Success = success, Message = message };
            }

            try
            {
                var job = _adminJobRepository.GetById(request.JobId);
                if (job == null)
                {
                    success = false;
                    message = $"Job with ID {request.JobId} not found.";
                    return new NotifyJobResponse { Success = success, Message = message };
                }

                // Handle job deletion
                _adminJobRepository.DeleteJob(request.JobId);

                success = true;
                message = $"Approved! Job with ID {request.JobId} deleted successfully.";
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error processing job deletion: {ex.Message}";
            }

            return new NotifyJobResponse { Success = success, Message = message };
        }

        public override async Task<NotifyJobResponse> NotifyJob(NotifyJobRequest request, ServerCallContext context)
        {
            string action = request.Action.ToLower();
            return action switch
            {
                "create" => await HandleCreateJob(request),
                "update" => await HandleUpdateJob(request),
                "delete" => await HandleDeleteJob(request),
                _ => new NotifyJobResponse
                {
                    Success = false,
                    Message = "Invalid action. Please specify 'Create', 'Update', or 'Delete'."
                }
            };
        }
    }
}

