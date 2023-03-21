using Application.Abstractions;
using Application.Commands.Notify;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Commands
{
    public class TestNotifyCommandHandler
    {
        private readonly Mock<IClientsService> _clientSevice;
        private readonly NotifyCommandHandler _sut;

        public TestNotifyCommandHandler()
        {
            _clientSevice = new Mock<IClientsService>();
            _sut = new NotifyCommandHandler(_clientSevice.Object);
        }

        [Fact]
        public async void Given_No_Subscribers_When_Handle_Then_ClientService_Send_NotCalled()
        {
            //Arrange
            var cmd = new NotifyCommand { Channel = "HELLO", Subscribers = null, Message = "WORLD" };

            //Act
            await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _clientSevice.Verify(X => X.SendAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async void Given_A_Subscriber_And_Empty_Message_When_Handle_Then_ClientService_Send_NotCalled()
        {
            //Arrange
            var cmd = new NotifyCommand { Channel = "HELLO", Subscribers = new HashSet<Guid>() { new Guid() }, Message = "" };

            //Act
            await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _clientSevice.Verify(X => X.SendAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async void Given_A_Subscriber_And_A_Message_When_Handle_Then_ClientService_Send_Called_Once()
        {
            //Arrange
            var subscriber = new Guid();
            var cmd = new NotifyCommand { Channel = "HELLO", Subscribers = new HashSet<Guid>() { subscriber }, Message = "HELLO" };

            //Act
            await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _clientSevice.Verify(X => X.SendAsync(subscriber, "CHN:HELLO|HELLO"), Times.Once());
        }

        [Fact]
        public async void Given_Some_Subscribers_And_A_Message_When_Handle_Then_ClientService_Send_Called_Nr_Of_Subscribers_Times()
        {
            //Arrange
            var subscriber1 = new Guid();
            var subscriber2 = new Guid();
            var set = new HashSet<Guid>() { subscriber1, subscriber2 };

            var cmd = new NotifyCommand { Channel = "HELLO", Subscribers = set, Message = "HELLO" };

            //Act
            await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _clientSevice.Verify(X => X.SendAsync(subscriber1, "CHN:HELLO|HELLO"), Times.Once());
            _clientSevice.Verify(X => X.SendAsync(subscriber2, "CHN:HELLO|HELLO"), Times.Once());
        }
    }
}