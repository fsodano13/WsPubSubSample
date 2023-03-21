using MediatR;

namespace Application.Commands.RemoveClient
{
    public record RemoveClientCommand : IRequest
    {
        public Guid Client { get; init; }
    }
}
