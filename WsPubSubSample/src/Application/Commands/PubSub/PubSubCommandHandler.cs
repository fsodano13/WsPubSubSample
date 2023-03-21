using Application.Abstractions;
using MediatR;
using Application.Models;

namespace Application.Commands.PubSub
{
    public class PubSubCommandHandler : IRequestHandler<PubSubCommand, PubSubCommandResponse>
    {
        private readonly IPubSubService _service;

        public PubSubCommandHandler(IPubSubService service)
        {
            _service = service;
        }

        public Task<PubSubCommandResponse> Handle(PubSubCommand command, CancellationToken cancellationToken)
        {
            string[]? args = command?.Data?.Split('|');
            var parsed = new
            {
                Command = args?.Length > 0 ? args[0] : string.Empty,
                Channel = args?.Length > 1 ? args[1] : string.Empty,
                Message = args?.Length > 2 ? args[2] : string.Empty,
            };

            if (string.IsNullOrEmpty(parsed.Command) || string.IsNullOrEmpty(parsed.Channel))
                return Task.FromResult(new PubSubCommandResponse { ErrorMessage = ErrorMessages.MalformedCommand, IsValid = false });

            switch (parsed.Command)
            {
                case "PUB":
                    if (string.IsNullOrEmpty(parsed.Message))
                        return Task.FromResult(new PubSubCommandResponse { ErrorMessage = ErrorMessages.UndefinedMessage, IsValid = false });
                    _service.Publish(parsed.Channel, parsed.Message, command!.Client);
                    break;

                case "SUB":
                    if (string.IsNullOrEmpty(parsed.Message))
                        _service.Subscribe(parsed.Channel, command!.Client);
                    else
                        return Task.FromResult(new PubSubCommandResponse { ErrorMessage = ErrorMessages.MalformedCommand, IsValid = false });
                    break;

                case "UNS":
                    if (string.IsNullOrEmpty(parsed.Message))
                        _service.Unsubscribe(parsed.Channel, command!.Client);
                    else
                        return Task.FromResult(new PubSubCommandResponse { ErrorMessage = ErrorMessages.MalformedCommand, IsValid = false });
                    break;

                default:
                    return Task.FromResult(new PubSubCommandResponse { ErrorMessage = ErrorMessages.UnknownCommand, IsValid = false });
            }

            return Task.FromResult(new PubSubCommandResponse { IsValid = true });
        }
    }
}