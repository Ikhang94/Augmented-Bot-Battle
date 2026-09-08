using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendESP.Controllers;
using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace BackendESP.Tests.Controllers
{
    public class PlayerControllerTests
    {
        private static PlayerController CreateController(
            Mock<IPlayerData> playerDataMock,
            Mock<ITokenService>? tokenServiceMock = null)
            => new PlayerController(
                playerDataMock.Object,
                (tokenServiceMock ?? new Mock<ITokenService>()).Object);

        [Fact]
        public async Task CreatePlayer_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            var player = new PlayerDTO();
            IResult expected = Results.Ok();

            var playerDataMock = new Mock<IPlayerData>(MockBehavior.Strict);
            playerDataMock
                .Setup(x => x.CreatePlayerAsync(player))
                .ReturnsAsync(expected);

            var controller = CreateController(playerDataMock);

            // Act
            var result = await controller.CreatePlayer(player);

            // Assert
            Assert.Same(expected, result);
            playerDataMock.Verify(x => x.CreatePlayerAsync(player), Times.Once);
            playerDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAllPlayers_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            IResult expected = Results.Ok();

            var playerDataMock = new Mock<IPlayerData>(MockBehavior.Strict);
            playerDataMock
                .Setup(x => x.GetAllPlayersAsync())
                .ReturnsAsync(expected);

            var controller = CreateController(playerDataMock);

            // Act
            var result = await controller.GetAllPlayers();

            // Assert
            Assert.Same(expected, result);
            playerDataMock.Verify(x => x.GetAllPlayersAsync(), Times.Once);
            playerDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetPlayerAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            var playerId = 42;
            IResult expected = Results.Ok();

            var playerDataMock = new Mock<IPlayerData>(MockBehavior.Strict);
            playerDataMock
                .Setup(x => x.GetPlayerAsync(playerId))
                .ReturnsAsync(expected);

            var controller = CreateController(playerDataMock);

            // Act
            var result = await controller.GetPlayerAsync(playerId);

            // Assert
            Assert.Same(expected, result);
            playerDataMock.Verify(x => x.GetPlayerAsync(playerId), Times.Once);
            playerDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdatePlayerAsync_Uses_First_And_Last_And_Delegates_To_DataLayer()
        {
            // Arrange
            var oldPlayer = new PlayerDTO();
            var newPlayer = new PlayerDTO();

            // Important pour couvrir les lignes:
            // PlayerDTO oldPlayer = playerDetails.First();
            // PlayerDTO newPlayer = playerDetails.Last();
            IEnumerable<PlayerDTO> playerDetails = new[] { oldPlayer, newPlayer };

            IResult expected = Results.Ok();

            var playerDataMock = new Mock<IPlayerData>(MockBehavior.Strict);
            playerDataMock
                .Setup(x => x.UpdatePlayerAsync(oldPlayer, newPlayer))
                .ReturnsAsync(expected);

            var controller = CreateController(playerDataMock);

            // Act
            var result = await controller.UpdatePlayerAsync(playerDetails);

            // Assert
            Assert.Same(expected, result);
            playerDataMock.Verify(x => x.UpdatePlayerAsync(oldPlayer, newPlayer), Times.Once);
            playerDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeletePlayerAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            var playerId = 7;
            IResult expected = Results.Ok();

            var playerDataMock = new Mock<IPlayerData>(MockBehavior.Strict);
            playerDataMock
                .Setup(x => x.DeletePlayerAsync(playerId))
                .ReturnsAsync(expected);

            var controller = CreateController(playerDataMock);

            // Act
            var result = await controller.DeletePlayerAsync(playerId);

            // Assert
            Assert.Same(expected, result);
            playerDataMock.Verify(x => x.DeletePlayerAsync(playerId), Times.Once);
            playerDataMock.VerifyNoOtherCalls();
        }
    }
}
