namespace Application.Abstractions
{
    public interface IClientsService
    {
        bool AddClient(IClient client);

        bool RemoveClient(Guid guid);

        Task SendAsync(Guid client, string message);
    }
}
