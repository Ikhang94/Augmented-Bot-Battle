using BackendESP.Models;
using Xunit;

namespace BackendESP.Tests.Models
{
    public class RobotTests
    {
        [Fact]
        public void Robot_Can_Set_And_Get_Scalar_Properties()
        {
            var r = new Robot
            {
                RobotId = 77,
                Name = "R2D2"
            };

            Assert.Equal(77, r.RobotId);
            Assert.Equal("R2D2", r.Name);
        }

        [Fact]
        public void Robot_Collections_Are_Initialized_By_Default()
        {
            var r = new Robot();

            Assert.NotNull(r.GamesessionClientRobots);
            Assert.NotNull(r.GamesessionHostRobots);
            Assert.NotNull(r.PlayersRobots);
        }

        [Fact]
        public void Robot_Collections_Can_Add_Items()
        {
            var r = new Robot();

            var sessionAsClient = new Gamesession();
            var sessionAsHost = new Gamesession();
            var playersRobot = new PlayersRobot();

            r.GamesessionClientRobots.Add(sessionAsClient);
            r.GamesessionHostRobots.Add(sessionAsHost);
            r.PlayersRobots.Add(playersRobot);

            Assert.Contains(sessionAsClient, r.GamesessionClientRobots);
            Assert.Contains(sessionAsHost, r.GamesessionHostRobots);
            Assert.Contains(playersRobot, r.PlayersRobots);
        }
    }
}
