using Microsoft.AspNetCore.Mvc;
using JobService.Domain.Services;
using JobService.Domain.Entities;

namespace JobService.Api.Controllers
{
    /// <summary>
    /// REST API endpoints for Job operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // POST: api/Job
        [HttpPost]
        public IActionResult PostJob([FromBody] Job job)
        {

            Console.WriteLine("CreateJob endpoint hit."); 

            if (job == null)
                return BadRequest("Job cannot be null.");
                
            // In a real scenario, you would also check the authenticated user's ID
            // For demonstration, assume OwnerId is passed
            var createdJob = _jobService.CreateJob(job);
            
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
        public IActionResult UpdateJob(int id, [FromBody] Job jobToUpdate, [FromQuery] string ownerId)
        {
            jobToUpdate.Id = id;
            var updatedJob = _jobService.UpdateJob(jobToUpdate, ownerId);
            if (updatedJob == null) return NotFound();
            return Ok(updatedJob);
        }

        // DELETE: api/Job/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteJob(int id, [FromQuery] string ownerId)
        {
            _jobService.DeleteJob(id, ownerId);
            return NoContent();
        }
    }
}
