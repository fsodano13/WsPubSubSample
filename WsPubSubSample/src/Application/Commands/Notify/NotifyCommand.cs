using MediatR;

namespace Application.Commands.Notify
{
    public record NotifyCommand : IRequest
    {
        public Guid Publisher { get; init; }

        public HashSet<Guid>? Subscribers { get; init; }

        public string? Channel { get; init; }

        public string? Message { get; init; }
    }
}
