using Application.Abstractions;
using Application.Commands.PubSub;
using FluentAssertions.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Tests.Integration.Application
{
    public class ApplicationTests : IClassFixture<WebApplicationFactory<Program>>

    {
        private const string _fooChannel = "FOO";
        private const string _barChannel = "BAR";
        private readonly WebApplicationFactory<Program> _factory;

        private IPubSubService _pubSubService;
        private IClientsService _clientService;
        private ISender _mediator;

        public ApplicationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            _mediator = scope.ServiceProvider.GetRequiredService<ISender>();
            _clientService = scope.ServiceProvider.GetRequiredService<IClientsService>();
            _pubSubService = scope.ServiceProvider.GetRequiredService<IPubSubService>();
        }

        [Fact]
        public async Task Given_A_Subscriber_When_Pub_Msg_Arrives_And_Client_Is_Subscribed_Then_Client_Receives_Msg()
        {
            var subGuid = Guid.NewGuid();
            var subMoq = new Mock<IClient>();
            subMoq.Setup(c => c.ClientId).Returns(subGuid);
            subMoq.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _clientService.AddClient(subMoq.Object);
            _pubSubService.SubscribeChannel(_fooChannel, subGuid);

            await _mediator.Send(new PubSubCommand
            {
                Data = $"PUB|{_fooChannel}|HELLO",
                Client = Guid.NewGuid(),
            });

            subMoq.Verify(X => X.SendAsync($"CHN:{_fooChannel}|HELLO"), Times.Once());
        }

        [Fact]
        public async Task Given_Two_Subscribers_When_Pub_Msg_Arrives_And_Only_A_Client_Is_Subscribed_Then_This_One_Receives_Msg()
        {
            var subGuid1 = Guid.NewGuid();
            var subGuid2 = Guid.NewGuid();

            var subMoq1 = new Mock<IClient>();
            subMoq1.Setup(c => c.ClientId).Returns(subGuid1);
            subMoq1.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            var subMoq2 = new Mock<IClient>();
            subMoq2.Setup(c => c.ClientId).Returns(subGuid2);
            subMoq2.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _clientService.AddClient(subMoq1.Object);
            _pubSubService.SubscribeChannel(_fooChannel, subGuid1);
            _clientService.AddClient(subMoq2.Object);
            _pubSubService.SubscribeChannel(_barChannel, subGuid2);

            await _mediator.Send(new PubSubCommand
            {
                Data = $"PUB|{_fooChannel}|HELLO",
                Client = Guid.NewGuid(),
            });

            subMoq1.Verify(X => X.SendAsync($"CHN:{_fooChannel}|HELLO"), Times.Once());
            subMoq2.Verify(X => X.SendAsync($"CHN:{_fooChannel}|HELLO"), Times.Never());
        }

        [Fact]
        public async Task Given_Two_Subscribers_When_Pub_Msg_Arrives_And_They_Are_Subscribed_Then_They_Receive_Msg()
        {
            var subGuid1 = Guid.NewGuid();
            var subGuid2 = Guid.NewGuid();

            var subMoq1 = new Mock<IClient>();
            subMoq1.Setup(c => c.ClientId).Returns(subGuid1);
            subMoq1.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            var subMoq2 = new Mock<IClient>();
            subMoq2.Setup(c => c.ClientId).Returns(subGuid2);
            subMoq2.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _clientService.AddClient(subMoq1.Object);
            _pubSubService.SubscribeChannel(_fooChannel, subGuid1);
            _clientService.AddClient(subMoq2.Object);
            _pubSubService.SubscribeChannel(_fooChannel, subGuid2);

            await _mediator.Send(new PubSubCommand
            {
                Data = $"PUB|{_fooChannel}|HELLO",
                Client = Guid.NewGuid(),
            });

            subMoq1.Verify(X => X.SendAsync($"CHN:{_fooChannel}|HELLO"), Times.Once());
            subMoq2.Verify(X => X.SendAsync($"CHN:{_fooChannel}|HELLO"), Times.Once());
        }

        [Fact]
        public async Task Given_A_Subscriber_When_Pub_Msg_Arrives_And_Client_Is_Subscribed_Then_Client_Does_Not_Receive_Msg()
        {
            var subGuid = Guid.NewGuid();
            var subMoq = new Mock<IClient>();
            subMoq.Setup(c => c.ClientId).Returns(subGuid);
            subMoq.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _clientService.AddClient(subMoq.Object);
            _pubSubService.SubscribeChannel(_fooChannel, subGuid);

            await _mediator.Send(new PubSubCommand
            {
                Data = $"PUB|{_barChannel}|HELLO",
                Client = Guid.NewGuid(),
            });

            subMoq.Verify(X => X.SendAsync($"CHN:{_fooChannel}|HELLO"), Times.Never());
        }

        [Fact]
        public async Task Given_A_Subscriber_When_Pub_Msg_Arrives_And_Client_Is_Unsubscribed_After_Subscription_Then_Client_Does_Not_Receive_Msg()
        {
            _clientService.Reset();
            _pubSubService.Reset();

            var subGuid = Guid.NewGuid();
            var subMoq = new Mock<IClient>();
            subMoq.Setup(c => c.ClientId).Returns(subGuid);
            subMoq.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _clientService.AddClient(subMoq.Object);
            _pubSubService.SubscribeChannel(_fooChannel, subGuid);
            _pubSubService.UnsubscribeChannel(_fooChannel, subGuid);

            await _mediator.Send(new PubSubCommand
            {
                Data = $"PUB|{_barChannel}|HELLO",
                Client = Guid.NewGuid(),
            });

            subMoq.Verify(X => X.SendAsync($"CHN:{_fooChannel}|HELLO"), Times.Never());
        }

        [Fact]
        public async Task Given_A_Client_And_No_Others_More_When_It_Sends_Brd_Msg_Then_This_Does_not_Receive_The_msg()
        {
            var subGuid1 = Guid.NewGuid();

            var subMoq1 = new Mock<IClient>();
            subMoq1.Setup(c => c.ClientId).Returns(subGuid1);
            subMoq1.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _clientService.AddClient(subMoq1.Object);

            await _mediator.Send(new PubSubCommand
            {
                Data = $"BRD|HELLO",
                Client = subGuid1,
            });

            subMoq1.Verify(X => X.SendAsync("BRD:HELLO"), Times.Never());
        }

        [Fact]
        public async Task Given_Some_Clients_When_Brd_Msg_Arrives_Then_All_The_Clients_Except_The_Publisher_Receive_Msg()
        {
            var subGuid1 = Guid.NewGuid();
            var subGuid2 = Guid.NewGuid();
            var subGuid3 = Guid.NewGuid();

            var subMoq1 = new Mock<IClient>();
            subMoq1.Setup(c => c.ClientId).Returns(subGuid1);
            subMoq1.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            var subMoq2 = new Mock<IClient>();
            subMoq2.Setup(c => c.ClientId).Returns(subGuid2);
            subMoq2.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            var subMoq3 = new Mock<IClient>();
            subMoq3.Setup(c => c.ClientId).Returns(subGuid3);
            subMoq3.Setup(c => c.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _clientService.AddClient(subMoq1.Object);
            _clientService.AddClient(subMoq2.Object);
            _clientService.AddClient(subMoq2.Object);

            await _mediator.Send(new PubSubCommand
            {
                Data = $"BRD|HELLO",
                Client = subGuid3,
            });

            subMoq1.Verify(X => X.SendAsync("BRD:HELLO"), Times.Once());
            subMoq2.Verify(X => X.SendAsync("BRD:HELLO"), Times.Once());
            subMoq3.Verify(X => X.SendAsync("BRD:HELLO"), Times.Never());
        }
    }
}