using Application.Abstractions;
using Application.Models;
using MediatR;

namespace Application.Commands.PubSub
{
    public class PubSubCommandHandler : IRequestHandler<PubSubCommand, PubSubCommandResponse>
    {
        private readonly IPubSubService _service;

        public PubSubCommandHandler(IPubSubService service)
        {
            _service = service;
        }

        public async Task<PubSubCommandResponse> Handle(PubSubCommand command, CancellationToken cancellationToken)
        {
            string[]? args = command?.Data?.Split('|');
            var parsed = new
            {
                Command = args?.Length > 0 ? args[0] : string.Empty,
                Param1 = args?.Length > 1 ? args[1] : string.Empty,
                Param2 = args?.Length > 2 ? args[2] : string.Empty,
            };

            if (string.IsNullOrEmpty(parsed.Command) || string.IsNullOrEmpty(parsed.Param1))
                return await Task.FromResult(new PubSubCommandResponse { ErrorMessage = ErrorMessages.MalformedCommand, IsValid = false });

            switch (parsed.Command)
            {
                case "PUB":
                    if (string.IsNullOrEmpty(parsed.Param2))
                        return new PubSubCommandResponse { ErrorMessage = ErrorMessages.UndefinedMessage, IsValid = false };
                    await _service.Publish(parsed.Param1, parsed.Param2, command!.Client);
                    break;

                case "SUB":
                    if (string.IsNullOrEmpty(parsed.Param2))
                        _service.SubscribeChannel(parsed.Param1, command!.Client);
                    else
                        return new PubSubCommandResponse { ErrorMessage = ErrorMessages.MalformedCommand, IsValid = false };
                    break;

                case "UNS":
                    if (string.IsNullOrEmpty(parsed.Param2))
                        _service.UnsubscribeChannel(parsed.Param1, command!.Client);
                    else
                        return new PubSubCommandResponse { ErrorMessage = ErrorMessages.MalformedCommand, IsValid = false };
                    break;

                case "BRD":
                    if (string.IsNullOrEmpty(parsed.Param1))
                        return new PubSubCommandResponse { ErrorMessage = ErrorMessages.MalformedCommand, IsValid = false };
                    await _service.BroadcastAll(parsed.Param1, command!.Client);

                    break;

                default:
                    return new PubSubCommandResponse { ErrorMessage = ErrorMessages.UnknownCommand, IsValid = false };
            }

            return new PubSubCommandResponse { IsValid = true };
        }
    }
}