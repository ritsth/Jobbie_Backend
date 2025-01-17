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
        private readonly AdminJobClient _adminJobClient;

        public AdminJobController(IAdminJobRepository adminJobRepository, AdminJobClient adminJobClient)
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

        // GET: api/AdminJob/{id}
        [HttpGet("{id}")]
        public IActionResult GetJobById(int id)
        {
            try
            {
                var job = _adminJobRepository.GetById(id);
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
            var request = new NotifyJobRequest
            {
                JobId = job.Id,
                Title = job.Title,
                Description = job.Description,
                Status = job.Status,
                OwnerId = job.OwnerId,
                CreatedAt = Timestamp.FromDateTime(job.CreatedDateTime), // Convert DateTime to Timestamp
                Action = "create"
            };

            _adminJobClient.CreateJobAsync(request);

            try
            {
                if (job == null)
                {
                    return BadRequest("Job data is null.");
                }

                var createdJob = _adminJobRepository.InsertJob(job);
                return CreatedAtAction(nameof(GetJobById), new { id = createdJob.Id }, createdJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/AdminJob/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateJob(int id, [FromBody] AdminJobEntity job)
        {
            //Send message to JOB Service about the job is Approved by Admin
            var request = new NotifyJobRequest
            {
                JobId = job.Id,
                Title = job.Title,
                Description = job.Description,
                Status = job.Status,
                OwnerId = job.OwnerId,
                CreatedAt = Timestamp.FromDateTime(job.CreatedDateTime), // Convert DateTime to Timestamp
                Action = "update"
            };

            _adminJobClient.UpdateJobAsync(request);

            try
            {
                if (job == null || job.Id != id)
                {
                    return BadRequest("Job data is invalid.");
                }

                var updatedJob = _adminJobRepository.UpdateJob(job);
                return Ok(updatedJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/AdminJob/{id}
        [HttpDelete("{id}")]
        public async IActionResult DeleteJob(int id)
        {
            await _adminJobClient.DeleteJobAsync(id);

            try
            {
                _adminJobRepository.DeleteJob(id);
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
