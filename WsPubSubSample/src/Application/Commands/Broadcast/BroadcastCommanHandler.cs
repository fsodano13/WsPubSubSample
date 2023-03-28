using Application.Abstractions;
using Application.Models;
using MediatR;

namespace Application.Commands.Broadcast
{
    public class BroadcastCommandHandler : IRequestHandler<BroadcastCommand>
    {
        private readonly IClientsService _service;

        public BroadcastCommandHandler(IClientsService service)
        {
            _service = service;
        }

        public async Task Handle(BroadcastCommand command, CancellationToken cancellationToken)
        {
            await _service.SendAllAsync($"BRD:{command.Data}", command.Publisher);
        }
    }
}