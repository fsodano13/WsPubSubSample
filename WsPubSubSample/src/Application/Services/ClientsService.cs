using System.Collections.Concurrent;
using Application.Abstractions;

namespace Application.Services
{
    public sealed class ClientsService : IClientsService
    {
        public ConcurrentDictionary<Guid, IClient> Clients { get; init; } = new ConcurrentDictionary<Guid, IClient>();

        public bool AddClient(IClient client) => Clients.TryAdd(client.ClientId, client);

        public bool RemoveClient(Guid guid) => Clients.Remove(guid, out _);

        public async Task SendAsync(Guid client, string message) =>
            await (Clients.GetValueOrDefault(client)?.SendAsync(message) ?? Task.CompletedTask);
    }
}
