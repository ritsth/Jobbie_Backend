using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using JobService.Domain.Repositories;
using JobService.Domain.Entities;
using JobService.Grpc.Protos;
using JobService.Kafka.Services;
using Microsoft.Extensions.Logging;

namespace JobService.Grpc.Services;

public class JobGrpcService : JobAdmin.JobAdminBase
{
    private readonly IJobRepository _jobRepository;
    private readonly KafkaProducer _kafkaProducer;
    private readonly ILogger<JobGrpcService> _logger;

    private const string ActionCreate = "create";
    private const string ActionUpdate = "update";
    private const string ActionDelete = "delete";

    public JobGrpcService(IJobRepository jobRepository, KafkaProducer kafkaProducer, ILogger<JobGrpcService> logger)
    {
        _jobRepository = jobRepository;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    private async Task NotifyKafkaAsync(Job job, string action)
    {
        var message = new
        {
            JobId = job.Id,
            Title = job.Title,
            Description = job.Description,
            Status = job.Status,
            OwnerId = job.OwnerId,
            CreatedAt = job.CreatedAt,
            Action = action
        };

        var serializedMessage = JsonSerializer.Serialize(message);
        _logger.LogInformation("Sending Kafka message: {Message}", serializedMessage);

        await _kafkaProducer.SendMessageAsync(job.Id.ToString(), message);

        _logger.LogInformation("Kafka message sent successfully for JobId: {JobId}, Action: {Action}", job.Id, action);
    }

    private NotifyJobResponse ValidateRequest(NotifyJobRequest request)
    {
        if (request == null)
        {
            _logger.LogWarning("Received null request.");
            return new NotifyJobResponse { Success = false, Message = "Request cannot be null." };
        }

        if (string.IsNullOrWhiteSpace(request.Action))
        {
            _logger.LogWarning("Request is missing the 'Action' field.");
            return new NotifyJobResponse { Success = false, Message = "Action is required." };
        }

        _logger.LogInformation("Validation passed for request: {Request}", JsonSerializer.Serialize(request));
        return new NotifyJobResponse { Success = true };
    }

    private async Task<NotifyJobResponse> HandleCreateJob(NotifyJobRequest request)
    {
        _logger.LogInformation("Processing 'Create' action for JobId: {JobId}", request.JobId);

        var createdAt = request.CreatedAt?.ToDateTime() ?? DateTime.UtcNow;

        var job = new Job
        {
            Id = request.JobId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            OwnerId = request.OwnerId,
            CreatedAt = createdAt
        };

        _jobRepository.InsertJob(job);
        _logger.LogInformation("Job created successfully: {Job}", JsonSerializer.Serialize(job));

        if (job.Status == "Approved")
        {
            _logger.LogInformation("Job approved. Sending 'Create' action to Kafka for JobId: {JobId}", job.Id);
            await NotifyKafkaAsync(job, ActionCreate);
        }

        return new NotifyJobResponse
        {
            Success = true,
            Message = "Job created successfully."
        };
    }

    private async Task<NotifyJobResponse> HandleUpdateJob(NotifyJobRequest request)
    {
        _logger.LogInformation("Processing 'Update' action for JobId: {JobId}", request.JobId);

        var job = _jobRepository.GetById(request.JobId);
        if (job == null)
        {
            _logger.LogWarning("Job with ID {JobId} not found.", request.JobId);
            return new NotifyJobResponse
            {
                Success = false,
                Message = "Job not found."
            };
        }

        job.Title = request.Title;
        job.Description = request.Description;
        job.Status = request.Status;
        _jobRepository.UpdateJob(job);
        _logger.LogInformation("Job updated successfully: {Job}", JsonSerializer.Serialize(job));

        if (job.Status == "Approved")
        {
            _logger.LogInformation("Job approved. Sending 'Update' action to Kafka for JobId: {JobId}", job.Id);
            await NotifyKafkaAsync(job, ActionUpdate);
        }

        return new NotifyJobResponse
        {
            Success = true,
            Message = "Job updated successfully."
        };
    }

    private async Task<NotifyJobResponse> HandleDeleteJob(NotifyJobRequest request)
    {
        _logger.LogInformation("Processing 'Delete' action for JobId: {JobId}", request.JobId);

        var job = _jobRepository.GetById(request.JobId);
        if (job == null)
        {
            _logger.LogWarning("Job with ID {JobId} not found.", request.JobId);
            return new NotifyJobResponse
            {
                Success = false,
                Message = "Job not found."
            };
        }

        _jobRepository.DeleteJob(job.Id);
        _logger.LogInformation("Job deleted successfully for JobId: {JobId}", job.Id);

        if (job.Status == "Approved")
        {
            _logger.LogInformation("Job approved. Sending 'Delete' action to Kafka for JobId: {JobId}", job.Id);
            await NotifyKafkaAsync(job, ActionDelete);
        }

        return new NotifyJobResponse
        {
            Success = true,
            Message = "Job deleted successfully."
        };
    }

    public override async Task<NotifyJobResponse> NotifyJob(NotifyJobRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received NotifyJob gRPC call with request: {Request}", JsonSerializer.Serialize(request));

        var validationResponse = ValidateRequest(request);
        if (!validationResponse.Success)
        {
            _logger.LogWarning("Validation failed for request: {Request}", JsonSerializer.Serialize(request));
            return validationResponse;
        }

        var response = request.Action.ToLower() switch
        {
            ActionCreate => await HandleCreateJob(request),
            ActionUpdate => await HandleUpdateJob(request),
            ActionDelete => await HandleDeleteJob(request),
            _ => new NotifyJobResponse
            {
                Success = false,
                Message = $"Unsupported action: {request.Action}"
            }
        };

        _logger.LogInformation("NotifyJob response: {Response}", JsonSerializer.Serialize(response));
        return response;
    }
}
