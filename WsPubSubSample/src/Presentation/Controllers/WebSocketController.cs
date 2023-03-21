using System.Net.WebSockets;
using Application.Commands.AddClient;
using Application.Commands.PubSub;
using Application.Commands.RemoveClient;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Clients;

namespace Presentation.Controllers;

public sealed class WebSocketController : ControllerBase
{
    private readonly ISender _mediator;

    public WebSocketController(ISender mediator)
    {
        _mediator = mediator;
    }

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var client = new WebSocketClient(webSocket);

            await _mediator.Send(new AddClientCommand
            {
                Client = client
            });

            await Echo(client);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Echo(WebSocketClient client)
    {
        try
        {
            var result = await client.ReceiveAsync();

            while (!result.CloseCommunication)
            {
                await ManageRequest(result, client.SendAsync);
                result = await client.ReceiveAsync();
            }
        }
        finally
        {
            await _mediator.Send(new RemoveClientCommand
            {
                Client = client.ClientId
            });
            await client.Ws.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                null,
                CancellationToken.None);
        }
    }

    private async Task ManageRequest(ClientRequest request, Func<string, Task> callback)
    {
        var response = await _mediator.Send(new PubSubCommand
        {
            Data = request.Data,
            Client = request.ClientId
        });

        await callback(response.IsValid ? "OK" : $"ERR|{response.ErrorMessage}");
    }
}
