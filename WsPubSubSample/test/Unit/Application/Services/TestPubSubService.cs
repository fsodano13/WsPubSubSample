using Application.Commands.Notify;
using Application.Services;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Services
{
    public class TestPubSubService
    {
        private readonly PubSubService _sut;
        private readonly Mock<ISender> _sender = new Mock<ISender>();

        public TestPubSubService()
        {
            _sut = new PubSubService(_sender.Object);
        }

        [Fact]
        public void When_Subscriber_Subscribes_For_The_First_Time_Returns_True()
        {
            //Arrange
            var subscriber = Guid.NewGuid();

            //Act
            var res = _sut.SubscribeChannel("HELLO", subscriber);

            //Assert
            res.Should().BeTrue();
            _sut.Channels.Should().HaveCount(1);
            _sut.Channels["HELLO"].Count.Should().Be(1);
        }

        [Fact]
        public void When_Subscriber_Subscribes_For_The_Second_Time_Returns_False()
        {
            //Arrange
            var subscriber = Guid.NewGuid();
            var res = _sut.SubscribeChannel("HELLO", subscriber);

            //Act
            res = _sut.SubscribeChannel("HELLO", subscriber);

            //Assert
            res.Should().BeFalse();
            _sut.Channels.Should().HaveCount(1);
            _sut.Channels["HELLO"].Count.Should().Be(1);
        }

        [Fact]
        public void More_Than_One_Subscriber_Can_Subscribe_The_Same_Channel()
        {
            //Arrange
            var subscriber = Guid.NewGuid();
            var subscriber2 = Guid.NewGuid();

            //Act
            var res = _sut.SubscribeChannel("HELLO", subscriber);
            res = _sut.SubscribeChannel("HELLO", subscriber2);

            //Assert
            res.Should().BeTrue();
            _sut.Channels.Should().HaveCount(1);
            _sut.Channels["HELLO"].Count.Should().Be(2);
        }

        [Fact]
        public void When_Subscriber_Unsubscribes_A_Subscribed_Channel_Returns_True()
        {
            //Arrange
            var subscriber = Guid.NewGuid();
            var subscriber2 = Guid.NewGuid();
            var res = _sut.SubscribeChannel("HELLO", subscriber);
            res = _sut.SubscribeChannel("HELLO", subscriber2);

            //Act
            res = _sut.UnsubscribeChannel("HELLO", subscriber);

            //Assert
            res.Should().BeTrue();
            _sut.Channels.Should().HaveCount(1);
            _sut.Channels["HELLO"].Count.Should().Be(1);
            _sut.Channels["HELLO"].Contains(subscriber).Should().BeFalse();
            _sut.Channels["HELLO"].Contains(subscriber2).Should().BeTrue();
        }

        [Fact]
        public void When_Subscriber_Unsubscribes_A_Not_Subscribed_Channel_Returns_False()
        {
            //Arrange
            var subscriber = Guid.NewGuid();
            var subscriber2 = Guid.NewGuid();
            var res = _sut.SubscribeChannel("HELLO", subscriber);

            //Act
            res = _sut.UnsubscribeChannel("HELLO", subscriber2);

            //Assert
            res.Should().BeFalse();
        }

        [Fact]
        public void When_Subscriber_Unsubscribes_All_No_Hashset_Has_ItsGuid()
        {
            //Arrange
            var subscriber = Guid.NewGuid();
            var subscriber2 = Guid.NewGuid();
            var res = _sut.SubscribeChannel("HELLO", subscriber);
            res = _sut.SubscribeChannel("HELLO", subscriber2);
            res = _sut.SubscribeChannel("HELLO2", subscriber);
            res = _sut.SubscribeChannel("HELLO2", subscriber2);
            res = _sut.SubscribeChannel("HELLO", subscriber);

            //Act
            _sut.Unsubscribe(subscriber);

            //Assert
            _sut.Channels.Where(c => c.Value.Contains(subscriber)).Count().Should().Be(0);
        }

        [Fact]
        public async Task Given_A_Not_Existing_Channel_When_Publish_Returns_False()
        {
            //Act
            var res = await _sut.Publish("HELLO", "WORD", Guid.NewGuid());

            //Assert
            res.Should().BeFalse();
        }

        [Fact]
        public async Task Given_A_Channel_With_NO_Subscribers_When_Publish_Returns_False()
        {
            //Arrange
            _sut.Channels.TryAdd("HELLO", new HashSet<Guid>());

            //Act
            var res = await _sut.Publish("HELLO", "WORD", Guid.NewGuid());

            //Assert
            res.Should().BeFalse();
        }

        [Fact]
        public async Task Given_A_Channel_With_A_Subscribers_When_Publish_Returns_True()
        {
            //Arrange
            var sub = Guid.NewGuid();
            var subscribersSet = new HashSet<Guid>() { sub };
            _sut.Channels.TryAdd("HELLO", subscribersSet);

            var pub = Guid.NewGuid();

            //Act
            var res = await _sut.Publish("HELLO", "WORLD", pub);

            //Assert
            res.Should().BeTrue();
            _sender.Verify(x => x.Send(It.Is<NotifyCommand>(c => c.Subscribers == subscribersSet &&
                                                            c.Publisher == pub &&
                                                            c.Channel == "HELLO" &&
                                                            c.Message == "WORLD"), CancellationToken.None));
        }
    }
}