## WsPubSubSample

WsPubSubSample is simplistic pub/sub implementation with a client and server in .Net Core 7.0. 
The client and server communicate using [WebSockets](https://learn.microsoft.com/it-it/aspnet/core/fundamentals/websockets?view=aspnetcore-7.0) over TCP.

The pub/sub implementation implements 3 commands:
1. A “publish” command that publishes a message to a channel.
2. A “subscribe” command which listens to a channel for messages.
2. A “unsubscribe” command which removes a subscription to a channel.

The channels are created on the fly during subscribe request, if not already existing.

The protocol for commands is quite simple. Clients can send almost 1024 ASCII characters.

The format is: CMD|CHANNEL|MESSAGE

Pipe character is used to separate each part of the payload.

CMD is 3 characters string. Digit PUB for PUBLISH, SUB for SUBSCRIBE, UNS for UNSUBSCRIBE. 
Pay attention, the protocol is case sensitive.

The remainder of the paylod can be as long as you like, but the total sum of the characters must fall within the maximum limit of the packet of 1024 characters, otherwhise message is truncated.

MESSAGE is required only for PUB commands.

## Examples
SUB|HELLO

UNS|HELLO

PUB|HELLO|WORLD


## Usage
Simply launch the application. You'll see a client web application that interacts with the server. 
To have more clients, you only need to open as many browser tabs as you want and digit the server [url](https://localhost:59590/).
If a problem opening the web socket at the indicated url is shown at launch, try changing the port number to a smaller one in src/Properties/launcSettings.json
(field applicationUrl).

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.


## License

[MIT](https://choosealicense.com/licenses/mit/)