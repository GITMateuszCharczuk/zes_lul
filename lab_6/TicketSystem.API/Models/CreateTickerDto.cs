namespace TicketSystem.API.Models
{
    public class CreateTicketDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public required string UserId { get; set; }
    }
} 