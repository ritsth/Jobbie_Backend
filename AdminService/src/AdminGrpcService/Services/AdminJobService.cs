using System;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using JobService.Grpc.Protos;
using AdminService.Infra.Repositories;
using AdminService.Infra.Entities;
using Microsoft.Extensions.Logging;

namespace AdminGrpcService.Services
{
    public class AdminJobService : JobAdmin.JobAdminBase
    {
        private readonly IAdminJobRepository _adminJobRepository;
        private readonly ILogger<AdminJobService> _logger;

        public AdminJobService(IAdminJobRepository adminJobRepository, ILogger<AdminJobService> logger)
        {
            _adminJobRepository = adminJobRepository;
            _logger = logger;
        }

        private async Task<NotifyJobResponse> HandleCreateJob(NotifyJobRequest request)
        {
            _logger.LogInformation("Received 'Create' job request: {Request}", request);

            bool success = false;
            string message;

            bool userResponse = true; // Simulated user approval

            if (!userResponse)
            {
                success = false;
                message = $"Admin declined the request to create '{request.Title}' job.";
                _logger.LogWarning(message);
                return new NotifyJobResponse { Success = success, Message = message };
            }

            try
            {
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
                _logger.LogInformation(message);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error processing job creation: {ex.Message}";
                _logger.LogError(ex, message);
            }

            return new NotifyJobResponse { Success = success, Message = message };
        }

        private async Task<NotifyJobResponse> HandleUpdateJob(NotifyJobRequest request)
        {
            _logger.LogInformation("Received 'Update' job request: {Request}", request);

            bool success = false;
            string message;

            bool userResponse = true; // Simulated user approval

            if (!userResponse)
            {
                success = false;
                message = $"Admin declined the request to update '{request.Title}' job.";
                _logger.LogWarning(message);
                return new NotifyJobResponse { Success = success, Message = message };
            }

            try
            {
                var job = _adminJobRepository.GetById(request.JobId);
                if (job == null)
                {
                    success = false;
                    message = $"Job with ID {request.JobId} not found.";
                    _logger.LogWarning(message);
                    return new NotifyJobResponse { Success = success, Message = message };
                }

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
                _logger.LogInformation(message);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error processing job update: {ex.Message}";
                _logger.LogError(ex, message);
            }

            return new NotifyJobResponse { Success = success, Message = message };
        }

        private async Task<NotifyJobResponse> HandleDeleteJob(NotifyJobRequest request)
        {
            _logger.LogInformation("Received 'Delete' job request: {Request}", request);

            bool success = false;
            string message;

            bool userResponse = true; // Simulated user approval

            if (!userResponse)
            {
                success = false;
                message = $"Admin declined the request to delete '{request.Title}' job.";
                _logger.LogWarning(message);
                return new NotifyJobResponse { Success = success, Message = message };
            }

            try
            {
                var job = _adminJobRepository.GetById(request.JobId);
                if (job == null)
                {
                    success = false;
                    message = $"Job with ID {request.JobId} not found.";
                    _logger.LogWarning(message);
                    return new NotifyJobResponse { Success = success, Message = message };
                }

                _adminJobRepository.DeleteJob(request.JobId);

                success = true;
                message = $"Approved! Job with ID {request.JobId} deleted successfully.";
                _logger.LogInformation(message);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Error processing job deletion: {ex.Message}";
                _logger.LogError(ex, message);
            }

            return new NotifyJobResponse { Success = success, Message = message };
        }

        public override async Task<NotifyJobResponse> NotifyJob(NotifyJobRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received NotifyJob gRPC call with action: {Action}", request.Action);

            string action = request.Action.ToLower();
            var response = action switch
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

            _logger.LogInformation("Response sent for action '{Action}': {Response}", request.Action, response);
            return response;
        }
    }
}
