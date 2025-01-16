using AdminService.Entities;
using AdminService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminJobController : ControllerBase
    {
        private readonly IAdminJobRepository _adminJobRepository;

        public AdminJobController(IAdminJobRepository adminJobRepository)
        {
            _adminJobRepository = adminJobRepository;
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
            //Send message to JOB Service about the job is created
            // var request = new NotifyJobRequest
            // {
            //     JobId = jobId,
            //     Title = title,
            //     Description = description,
            //     Status = status,
            //     OwnerId = ownerId,
            //     CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            //     Action = "update"
            // };

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
        public IActionResult DeleteJob(int id)
        {
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
