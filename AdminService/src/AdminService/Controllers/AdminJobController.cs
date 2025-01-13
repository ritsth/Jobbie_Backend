using Microsoft.AspNetCore.Mvc;
using AdminService.Repositories;
using AdminService.Entities;

namespace AdminService.Controllers
{
    [ApiController]
    [Route("jobs")]
    public class AdminJobController : ControllerBase
    {
        private readonly IAdminJobRepository _repository;

        public AdminJobController(IAdminJobRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _repository.GetAllJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _repository.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminJobEntity job)
        {
            await _repository.AddJobAsync(job);
            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AdminJobEntity job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }
            await _repository.UpdateJobAsync(job);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteJobAsync(id);
            return NoContent();
        }
    }
}
