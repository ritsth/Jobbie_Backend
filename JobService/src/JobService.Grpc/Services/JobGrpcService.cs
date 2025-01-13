using System.Threading.Tasks;
using Grpc.Core;
using JobService.Domain.Repositories;
using JobService.Domain.Entities;
using JobService.Grpc.Protos;

namespace JobService.Grpc.Services
{
    /// <summary>
    /// gRPC service that handles admin operations on jobs.
    /// </summary>
    public class JobGrpcService : JobAdmin.JobAdminBase
    {
        private readonly IJobRepository _jobRepository;

        public JobGrpcService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public override Task<ApproveJobResponse> ApproveJob(ApproveJobRequest request, ServerCallContext context)
        {
            // Use the PascalCase property "JobId"
            var job = _jobRepository.GetById(request.JobId);
            if (job == null)
            {
                return Task.FromResult(new ApproveJobResponse
                {
                    Success = false,
                    Message = "Job not found."
                });
            }

            job.Status = "Approved";
            _jobRepository.UpdateJob(job);

            // In a real scenario, you might also send an event to Kafka, etc.

            return Task.FromResult(new ApproveJobResponse
            {
                Success = true,
                Message = "Job approved successfully."
            });
        }

        public override Task<UpdateJobResponse> UpdateJob(UpdateJobRequest request, ServerCallContext context)
        {
            // Use "JobId", "Title", "Description", etc.
            var job = _jobRepository.GetById(request.JobId);
            if (job == null)
            {
                return Task.FromResult(new UpdateJobResponse
                {
                    Success = false,
                    Message = "Job not found."
                });
            }

            job.Title = request.Title;
            job.Description = request.Description;
            _jobRepository.UpdateJob(job);

            return Task.FromResult(new UpdateJobResponse
            {
                Success = true,
                Message = "Job updated successfully."
            });
        }

        public override Task<DeleteJobResponse> DeleteJob(DeleteJobRequest request, ServerCallContext context)
        {
            // Use "JobId", "Success", "Message"
            var job = _jobRepository.GetById(request.JobId);
            if (job == null)
            {
                return Task.FromResult(new DeleteJobResponse
                {
                    Success = false,
                    Message = "Job not found."
                });
            }

            _jobRepository.DeleteJob(job.Id);

            return Task.FromResult(new DeleteJobResponse
            {
                Success = true,
                Message = "Job deleted successfully."
            });
        }
    }
}
