using Application.Abstractions;
using Application.Commands.RemoveClient;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Commands
{
    public class TestRemoveClientCommandHandler
    {
        private readonly Mock<IClientsService> _clientSevice;
        private readonly Mock<IPubSubService> _pubSubService;

        private readonly RemoveClientCommandHandler _sut;

        public TestRemoveClientCommandHandler()
        {
            _clientSevice = new Mock<IClientsService>();
            _pubSubService = new Mock<IPubSubService>();
            _sut = new RemoveClientCommandHandler(_pubSubService.Object, _clientSevice.Object);
        }

        [Fact]
        public async void When_Handle_Then_Remove_Client_And_Unsubscribe_All_Channels()
        {
            //Arrange
            var client = new Guid();
            var cmd = new RemoveClientCommand { Client = client };

            //Act
            await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _clientSevice.Verify(X => X.RemoveClient(client));
            _pubSubService.Verify(X => X.UnsubscribeAll(client));
        }
    }
}