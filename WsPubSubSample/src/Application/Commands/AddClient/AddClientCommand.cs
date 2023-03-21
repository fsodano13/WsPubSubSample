using Application.Abstractions;
using MediatR;

namespace Application.Commands.AddClient
{
    public record AddClientCommand : IRequest
    {
        public IClient? Client { get; init; }
    }
}
