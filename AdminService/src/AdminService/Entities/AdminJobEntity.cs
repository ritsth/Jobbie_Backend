using System.ComponentModel.DataAnnotations;

namespace AdminService.Entities
{
    public class AdminJobEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        
        public string Status { get; set; } = "Pending"; // Possible statuses: Pending, Approved, Denied, Deleted

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "OwnerId must be between 1 and 50 characters.")]
        public string OwnerId { get; set; } = default!; // The user who created this job

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    }
}
