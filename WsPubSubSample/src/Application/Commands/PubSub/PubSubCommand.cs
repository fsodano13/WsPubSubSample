using MediatR;

namespace Application.Commands.PubSub
{
    public record PubSubCommand : IRequest<PubSubCommandResponse>
    {
        public Guid Client { get; init; }

        public string? Data { get; set; }
    }
}
