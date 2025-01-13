namespace AdminService.Entities
{
    public class AdminJobEntity
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public string Description { get; set; }

        public string status { get; set; } = "Pending"; //pending, approved, rejected, deleted

        public required int OwnerId { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    }
}
