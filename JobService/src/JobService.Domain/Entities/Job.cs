namespace JobService.Domain.Entities
{
    /// <summary>
    /// Represents a job entity in the system.
    /// </summary>
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        
        public string Status { get; set; } = "Pending"; 
        // Possible statuses: Pending, Approved, Denied, Deleted

        public string OwnerId { get; set; } = default!;
        // The user who created this job
    }
}
