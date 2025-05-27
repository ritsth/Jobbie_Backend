using AdminService.Infra.Entities;
using AdminService.Infra.Repositories;
using Microsoft.AspNetCore.Mvc;
using AdminService.Clients;
using JobService.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminJobController : ControllerBase
    {
        private readonly IAdminJobRepository _adminJobRepository;
        private readonly IAdminJobClient _adminJobClient;

        public AdminJobController(IAdminJobRepository adminJobRepository, IAdminJobClient adminJobClient)
        {
            _adminJobRepository = adminJobRepository;
            _adminJobClient = adminJobClient;
        }

        // GET: api/AdminJob
        [HttpGet]
        public IActionResult GetAllJobs()
        {
            try
            {
                var jobs = _adminJobRepository.GetAll();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/AdminJob/{jobId}
        [HttpGet("{jobId}")]
        public IActionResult GetJobById(string jobId)
        {
            try
            {
                var job = _adminJobRepository.GetById(jobId);
                if (job == null)
                {
                    return NotFound();
                }
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/AdminJob
        [HttpPost]
        public IActionResult CreateJob([FromBody] AdminJobEntity job)
        {
            //Send message to JOB Service about the job is Approved by Admin
            string newJobId = Guid.NewGuid().ToString();
            var request = new NotifyJobRequest
            {
                JobId = newJobId,
                Title = job.Title,
                Description = job.Description,
                Status = job.Status,
                OwnerId = job.OwnerId,
                CreatedAt = Timestamp.FromDateTime(job.CreatedDateTime), // Convert DateTime to Timestamp
                Action = "create"
            };



            try
            {
                if (job == null)
                {
                    return BadRequest("Job data is null.");
                }


                var createdJob = _adminJobRepository.InsertJob(job);
                _adminJobClient.CreateJobAsync(request);
                return CreatedAtAction(nameof(GetJobById), new { jobId = createdJob.JobId }, createdJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/AdminJob/{jobId}
        [HttpPut("{jobId}")]
        public IActionResult UpdateJob(string jobId, [FromBody] AdminJobEntity job)
        {
            //Send message to JOB Service about the job is Approved by Admin
            var request = new NotifyJobRequest
            {
                JobId = job.JobId,
                Title = job.Title,
                Description = job.Description,
                Status = job.Status,
                OwnerId = job.OwnerId,
                CreatedAt = Timestamp.FromDateTime(job.CreatedDateTime), // Convert DateTime to Timestamp
                Action = "update"
            };


            try
            {
                if (job == null || job.JobId != jobId)
                {
                    return BadRequest("Job data is invalid.");
                }

                var updatedJob = _adminJobRepository.UpdateJob(job);
                _adminJobClient.UpdateJobAsync(request);
                return Ok(updatedJob);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/AdminJob/{jobId}
        [HttpDelete("{jobId}")]
        public IActionResult DeleteJob(string jobId)
        {


            try
            {
                _adminJobRepository.DeleteJob(jobId);
                _adminJobClient.DeleteJobAsync(jobId);
                return NoContent(); // 204 No Content indicates the delete was successful
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/AdminJob/status/{status}
        [HttpGet("status/{status}")]
        public IActionResult GetJobsByStatus(string status)
        {
            try
            {
                var jobs = _adminJobRepository.GetByStatus(status);
                if (jobs == null || !jobs.Any())
                {
                    return NotFound();
                }
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
