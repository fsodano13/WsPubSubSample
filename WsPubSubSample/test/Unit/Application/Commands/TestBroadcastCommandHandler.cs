using Application.Abstractions;
using Application.Commands.Broadcast;
using Application.Commands.PubSub;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Commands
{
    public class TestBroadcastCommandHandler
    {
        private readonly Mock<IClientsService> _clientService;
        private readonly BroadcastCommandHandler _sut;

        public TestBroadcastCommandHandler()
        {
            _clientService = new Mock<IClientsService>();
            _sut = new BroadcastCommandHandler(_clientService.Object);
        }

        public async void Given_Broadcast_Cmd_When_Handle_Then_Notify_All_Clients()
        {
            //Arrange
            var cmd = new BroadcastCommand()
            {
                Data = "HELLO",
                Publisher = Guid.NewGuid()
            };

            //Act
            await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _clientService.Verify(X => X.SendAllAsync("HELLO", cmd.Publisher), Times.Once());
        }
    }
}