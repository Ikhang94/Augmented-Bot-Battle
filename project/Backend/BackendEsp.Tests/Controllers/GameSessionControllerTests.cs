using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendESP.Controllers;
using BackendESP.Data;
using BackendESP.DTO;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace BackendESP.Tests.Controllers
{
    public class GameSessionControllerTests
    {
        private static GameSessionController CreateController(Mock<IGameSessionData> gameSessionDataMock)
            => new GameSessionController(gameSessionDataMock.Object);

        [Fact]
        public async Task GetAllGameSessionsAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            IResult expected = Results.Ok();

            var gameSessionDataMock = new Mock<IGameSessionData>(MockBehavior.Strict);
            gameSessionDataMock
                .Setup(x => x.GetAllGameSessionsAsync())
                .ReturnsAsync(expected);

            var controller = CreateController(gameSessionDataMock);

            var result = await controller.GetAllGameSessionsAsync();

            Assert.Same(expected, result);
            gameSessionDataMock.Verify(x => x.GetAllGameSessionsAsync(), Times.Once);
            gameSessionDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetGameSessionAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            var sessionId = 42;
            IResult expected = Results.Ok();

            var gameSessionDataMock = new Mock<IGameSessionData>(MockBehavior.Strict);
            gameSessionDataMock
                .Setup(x => x.GetGameSessionAsync(sessionId))
                .ReturnsAsync(expected);

            var controller = CreateController(gameSessionDataMock);

            var result = await controller.GetGameSessionAsync(sessionId);

            Assert.Same(expected, result);
            gameSessionDataMock.Verify(x => x.GetGameSessionAsync(sessionId), Times.Once);
            gameSessionDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateGameSessionAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            var gameSession = new GameSessionDTO();
            IResult expected = Results.Ok();

            var gameSessionDataMock = new Mock<IGameSessionData>(MockBehavior.Strict);
            gameSessionDataMock
                .Setup(x => x.CreateGameSessionAsync(gameSession))
                .ReturnsAsync(expected);

            var controller = CreateController(gameSessionDataMock);

            var result = await controller.CreateGameSessionAsync(gameSession);

            Assert.Same(expected, result);
            gameSessionDataMock.Verify(x => x.CreateGameSessionAsync(gameSession), Times.Once);
            gameSessionDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateGameSessionAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            var gameSession = new GameSessionDTO();
            IResult expected = Results.Ok();

            var gameSessionDataMock = new Mock<IGameSessionData>(MockBehavior.Strict);
            gameSessionDataMock
                .Setup(x => x.UpdateGameSessionAsync(gameSession))
                .ReturnsAsync(expected);

            var controller = CreateController(gameSessionDataMock);

            var result = await controller.UpdateGameSessionAsync(gameSession);

            Assert.Same(expected, result);
            gameSessionDataMock.Verify(x => x.UpdateGameSessionAsync(gameSession), Times.Once);
            gameSessionDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteGameSessionAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            var sessionId = 7;
            IResult expected = Results.Ok();

            var gameSessionDataMock = new Mock<IGameSessionData>(MockBehavior.Strict);
            gameSessionDataMock
                .Setup(x => x.DeleteGameSessionAsync(sessionId))
                .ReturnsAsync(expected);

            var controller = CreateController(gameSessionDataMock);

            var result = await controller.DeleteGameSessionAsync(sessionId);

            Assert.Same(expected, result);
            gameSessionDataMock.Verify(x => x.DeleteGameSessionAsync(sessionId), Times.Once);
            gameSessionDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetGameSessionsOfPlayerAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            var playerId = 5;
            IResult expected = Results.Ok();

            var gameSessionDataMock = new Mock<IGameSessionData>(MockBehavior.Strict);
            gameSessionDataMock
                .Setup(x => x.GetGameSessionsOfPlayer(playerId))
                .ReturnsAsync(expected);

            var controller = CreateController(gameSessionDataMock);

            var result = await controller.GetGameSessionsOfPlayerAsync(playerId);

            Assert.Same(expected, result);
            gameSessionDataMock.Verify(x => x.GetGameSessionsOfPlayer(playerId), Times.Once);
            gameSessionDataMock.VerifyNoOtherCalls();
        }
    }
}
