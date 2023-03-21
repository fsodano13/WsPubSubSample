namespace Application.Abstractions
{
    public interface IClient
    {
        Guid ClientId { get; init; }

        Task SendAsync(string msg);
    }
}
