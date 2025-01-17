namespace TicketSystem.API.Models
{
    public class UpdateTicketDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public string? AdminComment { get; set; }
    }
} 