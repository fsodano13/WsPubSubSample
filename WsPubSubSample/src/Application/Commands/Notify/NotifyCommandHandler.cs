using Application.Abstractions;
using MediatR;

namespace Application.Commands.Notify
{
    public record NotifyCommandHandler : IRequestHandler<NotifyCommand>
    {
        private readonly IClientsService _service;

        public NotifyCommandHandler(IClientsService service)
        {
            _service = service;
        }

        public async Task Handle(NotifyCommand command, CancellationToken cancellationToken)
        {
            if (command.Subscribers?.Count > 0 && !string.IsNullOrEmpty(command.Message))
                await Parallel.ForEachAsync(command.Subscribers,
                                            async (client, cts) => await _service.SendAsync(client, $"CHN:{command.Channel}|{command.Message}"));
        }
    }
}