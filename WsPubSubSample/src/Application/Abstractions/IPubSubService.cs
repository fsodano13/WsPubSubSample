namespace Application.Abstractions
{
    public interface IPubSubService
    {
        Task<bool> Publish(string channel, string message, Guid publisher);

        bool SubscribeChannel(string channel, Guid subscriber);

        bool UnsubscribeChannel(string channel, Guid subscriber);

        void Unsubscribe(Guid subscriber);

        void Reset();

        Task<bool> BroadcastAll(string message, Guid publisher);
    }
}