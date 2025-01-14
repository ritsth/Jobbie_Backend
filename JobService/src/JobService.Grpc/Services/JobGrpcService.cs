using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using JobService.Domain.Repositories;
using JobService.Domain.Entities;
using JobService.Grpc.Protos;
using JobService.Kafka.Services;

namespace JobService.Grpc.Services
{
    public class JobGrpcService : JobAdmin.JobAdminBase
    {
        private readonly IJobRepository _jobRepository;
        private readonly KafkaProducer _kafkaProducer;

        public JobGrpcService(IJobRepository jobRepository, KafkaProducer kafkaProducer)
        {
            _jobRepository = jobRepository;
            _kafkaProducer = kafkaProducer;
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

            await _kafkaProducer.SendMessageAsync(job.Id.ToString(), message);
        }

        private async Task<NotifyJobResponse> HandleUpdateJob(NotifyJobRequest request)
        {
            var job = _jobRepository.GetById(request.JobId);
            if (job == null)
            {
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

            if (job.Status == "Approved")
            {
                await NotifyKafkaAsync(job, "Update");
            }

            return new NotifyJobResponse
            {
                Success = true,
                Message = "Job updated successfully."
            };
        }

        private async Task<NotifyJobResponse> HandleDeleteJob(NotifyJobRequest request)
        {
            var job = _jobRepository.GetById(request.JobId);
            if (job == null)
            {
                return new NotifyJobResponse
                {
                    Success = false,
                    Message = "Job not found."
                };
            }

            _jobRepository.DeleteJob(job.Id);

            if (job.Status == "Approved")
            {
                await NotifyKafkaAsync(job, "Delete");
            }

            return new NotifyJobResponse
            {
                Success = true,
                Message = "Job deleted successfully."
            };
        }

        private async Task<NotifyJobResponse> HandleCreateJob(NotifyJobRequest request)
        {
            var job = new Job
            {
                Id = request.JobId,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                OwnerId = request.OwnerId,
                CreatedAt = request.CreatedAt.ToDateTime()
            };

            _jobRepository.InsertJob(job);

            if (job.Status == "Approved")
            {
                await NotifyKafkaAsync(job, "Create");
            }

            return new NotifyJobResponse
            {
                Success = true,
                Message = "Job created successfully."
            };
        }

        public override async Task<NotifyJobResponse> NotifyJob(NotifyJobRequest request, ServerCallContext context)
        {
            return request.Action.ToLower() switch
            {
                "create" => await HandleCreateJob(request),
                "update" => await HandleUpdateJob(request),
                "delete" => await HandleDeleteJob(request),
                _ => new NotifyJobResponse
                {
                    Success = false,
                    Message = $"Unsupported action: {request.Action}"
                }
            };
        }
    }
}