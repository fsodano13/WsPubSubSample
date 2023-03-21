using System.Net.WebSockets;
using System.Text;
using Application.Abstractions;
using Application.Models;

namespace Presentation.Clients
{
    public sealed class WebSocketClient : IClient
    {
        public Guid ClientId { get; init; }
        public WebSocket Ws { get; init; }

        public WebSocketClient(WebSocket ws)
        {
            ClientId = Guid.NewGuid();
            Ws = ws;
        }

        public async Task SendAsync(string txt)
        {
            var bytes = Encoding.ASCII.GetBytes(txt);
            await Ws.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None);
        }

        public async Task<ClientRequest> ReceiveAsync()
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await Ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var content = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);
            return new ClientRequest
            {
                ClientId = ClientId,
                Data = content,
                CloseCommunication = receiveResult.CloseStatus.HasValue
            };
        }
    }
}
