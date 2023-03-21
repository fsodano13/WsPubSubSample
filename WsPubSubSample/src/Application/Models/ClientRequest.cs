namespace Application.Models
{
    public record ClientRequest
    {
        public Guid ClientId { get; set; }
        public string? Data { get; init; }

        public bool CloseCommunication { get; internal set; }
    }
}
