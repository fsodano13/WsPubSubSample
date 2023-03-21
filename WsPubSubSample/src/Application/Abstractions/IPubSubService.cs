namespace Application.Abstractions
{
    public interface IPubSubService
    {
        Task<bool> Publish(string channel, string message, Guid publisher);

        bool Subscribe(string channel, Guid subscriber);

        bool Unsubscribe(string channel, Guid subscriber);

        void UnsubscribeAll(Guid subscriber);
    }
}
