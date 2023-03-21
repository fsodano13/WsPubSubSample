using Application.Abstractions;
using Application.Commands.PubSub;
using FluentAssertions;
using Moq;
using Application.Models;
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

        [Fact]
        public async void Given_Invalid_String_When_Handle_Then_Return_Malformed_Command()
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = "AAA BBB"
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
            res.ErrorMessage.Should().Be(ErrorMessages.MalformedCommand);
        }

        [Fact]
        public async void Given_Unknown_Command_When_Handle_Then_Return_Unknown_Command()
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = "XXX|CHANNEL1"
            };

            //Act
            var res = await _sut.Handle(cmd, CancellationToken.None);

            //Assert
            res.IsValid.Should().BeFalse();
            res.ErrorMessage.Should().Be(ErrorMessages.UnknownCommand);
        }

        [Fact]
        public async void Given_Missing_Channel_When_Handle_Then_Return_Malformed_Command()
        {
            //Arrange
            var cmd = new PubSubCommand
            {
                Data = "SUB|"
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
            _pubSubSevice.Verify(X => X.Subscribe("CHANNEL1", cmd.Client), Times.Once());
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
            _pubSubSevice.Verify(X => X.Unsubscribe("CHANNEL1", cmd.Client), Times.Once());
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
    }
}