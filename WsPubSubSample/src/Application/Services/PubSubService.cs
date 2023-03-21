using System.Collections.Concurrent;
using Application.Abstractions;
using Application.Commands.Notify;
using MediatR;

namespace Application.Services
{
    public sealed class PubSubService : IPubSubService
    {
        private readonly ISender _mediator;

        public ConcurrentDictionary<string, HashSet<Guid>> Channels { get; init; }
            = new ConcurrentDictionary<string, HashSet<Guid>>();

        public PubSubService(ISender mediator)
        {
            _mediator = mediator;
        }

        public bool Subscribe(string channel, Guid subscriber) =>
            Channels.ContainsKey(channel) ?
            Channels[channel].Add(subscriber) :
            Channels.TryAdd(channel, new HashSet<Guid> { subscriber });

        public bool Unsubscribe(string channel, Guid subscriber) =>
            Channels[channel].Remove(subscriber);

        public void UnsubscribeAll(Guid subscriber) =>
            Parallel.ForEach(Channels.Keys, (key, channel) => Channels[key].Remove(subscriber));

        public async Task<bool> Publish(string channel, string message, Guid publisher)
        {
            if (Channels.ContainsKey(channel) && Channels[channel].Count() > 0)
            {
                await _mediator.Send(new NotifyCommand
                {
                    Message = message,
                    Publisher = publisher,
                    Channel = channel,
                    Subscribers = Channels[channel]
                });

                return true;
            }
            return false;
        }
    }
}
