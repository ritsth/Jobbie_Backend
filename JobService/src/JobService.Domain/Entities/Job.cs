using System.ComponentModel.DataAnnotations;

namespace JobService.Domain.Entities
{
    /// <summary>
    /// Represents a job entity in the system.
    /// </summary>
    public class Job
    {
        [Key]
        public string  JobId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        
        public string Status { get; set; } = "Pending"; // Possible statuses: Pending, Approved, Denied, Deleted

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "OwnerId must be between 1 and 50 characters.")]
        public string OwnerId { get; set; } = default!; // The user who created this job

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
