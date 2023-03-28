namespace Application.Commands.PubSub
{
    public record PubSubCommandResponse
    {
        public bool IsValid { get; set; }

        public string? ErrorMessage { get; init; }
    }
}