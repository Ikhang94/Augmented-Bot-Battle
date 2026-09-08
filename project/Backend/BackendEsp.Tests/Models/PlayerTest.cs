using BackendESP.Models;
using Xunit;

namespace BackendESP.Tests.Models
{
    public class PlayerTests
    {
        [Fact]
        public void Player_Can_Set_And_Get_Scalar_Properties()
        {
            var p = new Player
            {
                PlayerId = 42,
                Username = "alice",
                Email = "alice@example.com",
                Password = "secret"
            };

            Assert.Equal(42, p.PlayerId);
            Assert.Equal("alice", p.Username);
            Assert.Equal("alice@example.com", p.Email);
            Assert.Equal("secret", p.Password);
        }

        [Fact]
        public void Player_Collections_Are_Initialized_By_Default()
        {
            var p = new Player();

            Assert.NotNull(p.GamesessionClientPlayers);
            Assert.NotNull(p.GamesessionHostPlayers);
            Assert.NotNull(p.Leaderboards);
            Assert.NotNull(p.PlayersRobots);
        }

        [Fact]
        public void Player_Collections_Can_Add_Items()
        {
            var p = new Player();

            var sessionAsClient = new Gamesession();
            var sessionAsHost = new Gamesession();
            var leaderboard = new Leaderboard();
            var playersRobot = new PlayersRobot();

            p.GamesessionClientPlayers.Add(sessionAsClient);
            p.GamesessionHostPlayers.Add(sessionAsHost);
            p.Leaderboards.Add(leaderboard);
            p.PlayersRobots.Add(playersRobot);

            Assert.Contains(sessionAsClient, p.GamesessionClientPlayers);
            Assert.Contains(sessionAsHost, p.GamesessionHostPlayers);
            Assert.Contains(leaderboard, p.Leaderboards);
            Assert.Contains(playersRobot, p.PlayersRobots);
        }
    }
}
