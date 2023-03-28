using MediatR;

namespace Application.Commands.Broadcast
{
    public class BroadcastCommand : IRequest
    {
        public string Data { get; set; }
        public Guid Publisher { get; set; }
    }
}