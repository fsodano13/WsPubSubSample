using Application.Abstractions;
using Application.Commands.Broadcast;
using Application.Commands.PubSub;
using Application.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Commands
{
    public class TestPubSubCommandHandler
    {
        private readonly Mock<IPubSubService> _pubSubSevice;
        private readonly PubSubCommandHandler _sut;

        public TestPubSubCommandHandler()
        {
            _pubSubSevice = new Mock<IPubSubService>();
            _sut = new PubSubCommandHandler(_pubSubSevice.Object);
        }

        [Fact]
        public async void Given_Empty_Data_When_Handle_Then_Return_Malformed_Command()
        {
            //Arrange
            var cmd = new PubSubCommand();

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
            res.ErrorMessage.Should().Be(ErrorMessages.MalformedCommand);
        }

        [Theory]
        [InlineData("AAA BBB")]
        [InlineData("AAA")]
        public async void Given_Invalid_String_When_Handle_Then_Return_Malformed_Command(string data)
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = data
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
            res.ErrorMessage.Should().Be(ErrorMessages.MalformedCommand);
        }

        [Theory]
        [InlineData("XXX|CHANNEL1")]
        [InlineData("sub|AAA")]
        [InlineData("pub|AAA")]
        public async void Given_Unknown_Command_When_Handle_Then_Return_Unknown_Command(string data)
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = data
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
            res.ErrorMessage.Should().Be(ErrorMessages.UnknownCommand);
        }

        [Theory]
        [InlineData("SUB|")]
        [InlineData("SUB")]
        [InlineData("UNS|")]
        [InlineData("UNS")]
        public async void Given_Missing_Channel_When_Handle_SUB_or_PUB_Command_Then_Return_Malformed_Command(string data)
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = data
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
            res.ErrorMessage.Should().Be(ErrorMessages.MalformedCommand);
        }

        [Fact]
        public async void Given_Missing_Message_When_Handle_PUB_Command_Then_Return_Undefined_Message()
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = "PUB|CHANNEL1"
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
            res.ErrorMessage.Should().Be(ErrorMessages.UndefinedMessage);
        }

        [Fact]
        public async void Given_Valid_SUB_Message_When_Handle_Command_Then_Return_IsValid()
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = "SUB|CHANNEL1",
                Client = new Guid()
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _pubSubSevice.Verify(X => X.SubscribeChannel("CHANNEL1", cmd.Client), Times.Once());
            res.IsValid.Should().BeTrue();
        }

        [Fact]
        public async void Given_Valid_UNS_Message_When_Handle_Then_Return_IsValid()
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = "UNS|CHANNEL1",
                Client = new Guid()
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _pubSubSevice.Verify(X => X.UnsubscribeChannel("CHANNEL1", cmd.Client), Times.Once());
            res.IsValid.Should().BeTrue();
        }

        [Fact]
        public async void Given_Valid_PUB_Message_When_Handle_Then_Return_IsValid()
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = "PUB|CHANNEL1|HELLO",
                Client = new Guid()
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            _pubSubSevice.Verify(X => X.Publish("CHANNEL1", "HELLO", cmd.Client), Times.Once());
            res.IsValid.Should().BeTrue();
        }

        [Fact]
        public async void Given_Broadcast_Cmd_When_Handle_Then_Return_Is_Valid()
        {
            //Arrange
            var cmd = new PubSubCommand()
            {
                Data = "BRD|HELLO"
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeTrue();
        }

        [Fact]
        public async void Given_Broadcast_Cmd_When_Handle_And_Msg_Undefined_Then_Message_is_NotValid()
        {
            //Arrange
            var cmd = new PubSubCommand()
            {
                Data = "BRD|"
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
        }
    }
}