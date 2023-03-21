using Application.Abstractions;
using Application.Commands.AddClient;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Commands
{
    public class TestAddClientCommandHandler
    {
        private readonly Mock<IClientsService> _clientSevice;

        private readonly AddClientCommandHandler _sut;

        public TestAddClientCommandHandler()
        {
            _clientSevice = new Mock<IClientsService>();
            _sut = new AddClientCommandHandler(_clientSevice.Object);
        }

        [Fact]
        public async void When_Handle_Then_Require_Add_Client()
        {
            //Arrange
            var cmd = new AddClientCommand { Client = new Mock<IClient>().Object };

            //Act
            await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _clientSevice.Verify(X => X.AddClient(It.IsAny<IClient>()));
        }
    }
}