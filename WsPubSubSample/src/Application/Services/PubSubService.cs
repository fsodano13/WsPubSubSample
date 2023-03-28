using Application.Abstractions;
using Application.Commands.Broadcast;
using Application.Commands.Notify;
using MediatR;
using System.Collections.Concurrent;
using static System.Net.WebRequestMethods;

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

        public bool SubscribeChannel(string channel, Guid subscriber) =>
            Channels.ContainsKey(channel) ?
            Channels[channel].Add(subscriber) :
            Channels.TryAdd(channel, new HashSet<Guid> { subscriber });

        public bool UnsubscribeChannel(string channel, Guid subscriber) =>
            Channels[channel].Remove(subscriber);

        public void Unsubscribe(Guid subscriber) =>
            Parallel.ForEach(Channels.Keys, (key, channel) => Channels[key].Remove(subscriber));

        public async Task<bool> Publish(string channel, string message, Guid publisher)
        {
            if (Channels.ContainsKey(channel))
            {
                HashSet<Guid> subscribers = Channels[channel].Where(c => c != publisher).ToHashSet();
                if (subscribers.Count > 0)
                {
                    await _mediator.Send(new NotifyCommand
                    {
                        Message = message,
                        Publisher = publisher,
                        Channel = channel,
                        Subscribers = subscribers,
                    });

                    return true;
                }
            }
            return false;
        }

        public void Reset() => Channels.Clear();

        public async Task<bool> BroadcastAll(string message, Guid publisher)
        {
            await _mediator.Send(new BroadcastCommand
            {
                Data = message,
                Publisher = publisher,
            });

            return true;
        }
    }
}