using Microsoft.AspNetCore.Mvc;
using JobService.Domain.Services;
using JobService.Domain.Entities;
using JobService.Grpc.Protos; 
using Google.Protobuf.WellKnownTypes; 
using System.Threading.Tasks;

namespace JobService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        private readonly JobAdmin.JobAdminClient _jobAdminClient;

        public JobController(IJobService jobService, JobAdmin.JobAdminClient jobAdminClient)
        {
            _jobService = jobService;
            _jobAdminClient = jobAdminClient; // <-- store injected gRPC client
        }

        // POST: api/Job
        [HttpPost]
        // CHANGED: use async for gRPC call
        public async Task<IActionResult> PostJob([FromBody] Job job)
        {
            Console.WriteLine("CreateJob endpoint hit.");

            if (job is null)
                return BadRequest("Job cannot be null.");
            
            // Create the job in the database
            var createdJob = _jobService.CreateJob(job);

            // Notify AdminService via gRPC
            var grpcResponse = await _jobAdminClient.NotifyJobAsync(new NotifyJobRequest
            {
                JobId = createdJob.Id,
                Title = createdJob.Title,
                Description = createdJob.Description,
                Status = createdJob.Status,
                OwnerId = createdJob.OwnerId,
                CreatedAt = Timestamp.FromDateTime(System.DateTime.UtcNow),
                Action = "Create"
            });

            if (!grpcResponse.Success)
                return Problem($"Failed to notify admin service: {grpcResponse.Message}");

            // Return standard REST CreatedAtAction response
            return CreatedAtAction(nameof(GetJobById), new { id = createdJob.Id }, createdJob);
        }

        // GET: api/Job/{id}
        [HttpGet("{id}")]
        public IActionResult GetJobById(int id)
        {
            var job = _jobService.GetJobById(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        // GET: api/Job
        [HttpGet]
        public IActionResult GetAllJobs()
        {
            var jobs = _jobService.GetAllJobs();
            return Ok(jobs);
        }

        // GET: api/Job/status/{status}
        [HttpGet("status/{status}")]
        public IActionResult GetJobsByStatus(string status)
        {
            var jobs = _jobService.GetJobsByStatus(status);
            return Ok(jobs);
        }

        // PUT: api/Job/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] Job jobToUpdate, [FromQuery] string ownerId)
        {
            jobToUpdate.Id = id;

            // Update the job in the database
            var updatedJob = _jobService.UpdateJob(jobToUpdate, ownerId);
            if (updatedJob == null) return NotFound();

            // Notify AdminService via gRPC
            var grpcResponse = await _jobAdminClient.NotifyJobAsync(new NotifyJobRequest
            {
                JobId = updatedJob.Id,
                Title = updatedJob.Title,
                Description = updatedJob.Description,
                Status = updatedJob.Status,
                OwnerId = updatedJob.OwnerId,
                CreatedAt = Timestamp.FromDateTime(System.DateTime.UtcNow),
                Action = "Update"
            });

            if (!grpcResponse.Success)
                return Problem($"Failed to notify admin service: {grpcResponse.Message}");

            return Ok(updatedJob);
        }

        // DELETE: api/Job/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id, [FromQuery] string ownerId)
        {
            _jobService.DeleteJob(id, ownerId);


            // Notify AdminService via gRPC
            var grpcResponse = await _jobAdminClient.NotifyJobAsync(new NotifyJobRequest
            {
                JobId = id,
                CreatedAt = Timestamp.FromDateTime(System.DateTime.UtcNow),
                Action = "Delete"
            });

            if (!grpcResponse.Success)
                return Problem($"Failed to notify admin service: {grpcResponse.Message}");

            return NoContent();
        }
    }
}
