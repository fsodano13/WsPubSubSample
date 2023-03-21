using Application.Abstractions;
using Application.Commands.AddClient;

using Application.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Services
{
    public class TestClientsService
    {
        private readonly ClientsService _sut;

        public TestClientsService()
        {
            _sut = new ClientsService();
        }

        [Fact]
        public void When_Add_Client_For_The_First_Time_Returns_True()
        {
            //Arrange
            var client = new Mock<IClient>().Object;
            var key = client.ClientId;

            //Act
            var res = _sut.AddClient(client);

            //Assert
            res.Should().BeTrue();
            _sut.Clients.ContainsKey(key).Should().BeTrue();
        }

        [Fact]
        public void When_Add_Client_Twice_Second_One_Returns_False()
        {
            //Arrange
            var client = new Mock<IClient>().Object;
            var key = client.ClientId;

            //Act
            _sut.AddClient(client);
            var res = _sut.AddClient(client);

            //Assert
            res.Should().BeFalse();
            _sut.Clients.ContainsKey(key).Should().BeTrue();
        }

        [Fact]
        public void When_Remove_Client_Not_In_Dictionary_Returns_False()
        {
            //Arrange
            var client = new Mock<IClient>().Object;
            var key = client.ClientId;

            //Act
            var res = _sut.RemoveClient(key);

            //Assert
            res.Should().BeFalse();
            _sut.Clients.ContainsKey(key).Should().BeFalse();
        }

        [Fact]
        public void When_Remove_Client_In_Dictionary_Returns_True()
        {
            //Arrange
            var client = new Mock<IClient>().Object;
            var key = client.ClientId;

            //Act
            _sut.AddClient(client);
            var res = _sut.AddClient(client);
            res = _sut.RemoveClient(key);

            //Assert
            res.Should().BeTrue();
            _sut.Clients.ContainsKey(key).Should().BeFalse();
        }
    }
}