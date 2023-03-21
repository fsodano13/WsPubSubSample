using Application.Abstractions;
using MediatR;

namespace Application.Commands.AddClient
{
    public record AddClientCommandHandler : IRequestHandler<AddClientCommand>
    {
        private readonly IClientsService _clientsService;

        public AddClientCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        public Task Handle(AddClientCommand command, CancellationToken cancellationToken)
        {
            if (command.Client is not null)
                _clientsService.AddClient(command.Client);
            return Task.CompletedTask;
        }
    }
}
