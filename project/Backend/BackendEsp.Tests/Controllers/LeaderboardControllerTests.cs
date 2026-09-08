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
    public class LeaderboardControllerTests
    {
        private static LeaderboardController CreateController(Mock<ILeaderboardData> leaderboardDataMock)
            => new LeaderboardController(leaderboardDataMock.Object);

        [Fact]
        public async Task GetLeaderboard_Delegates_To_DataLayer_And_Returns_IResult()
        {
            IResult expected = Results.Ok();

            var leaderboardDataMock = new Mock<ILeaderboardData>(MockBehavior.Strict);
            leaderboardDataMock
                .Setup(x => x.GetLeaderboard())
                .ReturnsAsync(expected);

            var controller = CreateController(leaderboardDataMock);

            var result = await controller.GetLeadeoard();

            Assert.Same(expected, result);
            leaderboardDataMock.Verify(x => x.GetLeaderboard(), Times.Once);
            leaderboardDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateLeaderboard_Delegates_To_DataLayer_And_Returns_IResult()
        {
            var playerId = 42;
            IResult expected = Results.Ok();

            var leaderboardDataMock = new Mock<ILeaderboardData>(MockBehavior.Strict);
            leaderboardDataMock
                .Setup(x => x.CreateLeaderboard(playerId))
                .ReturnsAsync(expected);

            var controller = CreateController(leaderboardDataMock);

            var result = await controller.CreateLeaderboard(playerId);

            Assert.Same(expected, result);
            leaderboardDataMock.Verify(x => x.CreateLeaderboard(playerId), Times.Once);
            leaderboardDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SwapLeaderboard_WithTwoPlayers_Delegates_To_DataLayer_And_Returns_IResult()
        {
            var player1 = new LeaderboardDTO { PlayerId = 1, Rank = 1 };
            var player2 = new LeaderboardDTO { PlayerId = 2, Rank = 2 };
            var players = new List<LeaderboardDTO> { player1, player2 };

            IResult expected = Results.Ok();

            var leaderboardDataMock = new Mock<ILeaderboardData>(MockBehavior.Strict);
            leaderboardDataMock
                .Setup(x => x.SwapLeaderboard(player1, player2))
                .ReturnsAsync(expected);

            var controller = CreateController(leaderboardDataMock);

            var result = await controller.SwapLeaderboard(players);

            Assert.Same(expected, result);
            leaderboardDataMock.Verify(x => x.SwapLeaderboard(player1, player2), Times.Once);
            leaderboardDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SwapLeaderboard_WithLessThanTwoPlayers_ReturnsBadRequest()
        {
            var players = new List<LeaderboardDTO> { new LeaderboardDTO { PlayerId = 1, Rank = 1 } };

            var leaderboardDataMock = new Mock<ILeaderboardData>(MockBehavior.Strict);
            var controller = CreateController(leaderboardDataMock);

            var result = await controller.SwapLeaderboard(players);

            Assert.NotNull(result);
            leaderboardDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SwapLeaderboard_WithMoreThanTwoPlayers_ReturnsBadRequest()
        {
            var players = new List<LeaderboardDTO>
            {
                new LeaderboardDTO { PlayerId = 1, Rank = 1 },
                new LeaderboardDTO { PlayerId = 2, Rank = 2 },
                new LeaderboardDTO { PlayerId = 3, Rank = 3 }
            };

            var leaderboardDataMock = new Mock<ILeaderboardData>(MockBehavior.Strict);
            var controller = CreateController(leaderboardDataMock);

            var result = await controller.SwapLeaderboard(players);

            Assert.NotNull(result);
            leaderboardDataMock.VerifyNoOtherCalls();
        }
    }
}
