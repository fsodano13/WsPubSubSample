using Application.Abstractions;
using MediatR;

namespace Application.Commands.RemoveClient
{
    public record RemoveClientCommandHandler : IRequestHandler<RemoveClientCommand>
    {
        private readonly IPubSubService _pubSubService;
        private readonly IClientsService _clientsService;

        public RemoveClientCommandHandler(IPubSubService pubSubService, IClientsService clientsService)
        {
            _clientsService = clientsService;
            _pubSubService = pubSubService;
        }

        public Task Handle(RemoveClientCommand command, CancellationToken cancellationToken)
        {
            _pubSubService.Unsubscribe(command.Client);
            _clientsService.RemoveClient(command.Client);
            return Task.CompletedTask;
        }
    }
}