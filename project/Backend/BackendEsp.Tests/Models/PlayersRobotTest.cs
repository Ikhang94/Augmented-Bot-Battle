using BackendESP.Models;
using Xunit;

namespace BackendESP.Tests.Models
{
    public class PlayersRobotTests
    {
        [Fact]
        public void PlayersRobot_Can_Set_And_Get_Scalar_Properties()
        {
            var pr = new PlayersRobot
            {
                Id = 123,
                PlayerId = 10,
                RobotId = 20
            };

            Assert.Equal(123, pr.Id);
            Assert.Equal(10, pr.PlayerId);
            Assert.Equal(20, pr.RobotId);
        }

        [Fact]
        public void PlayersRobot_Can_Set_And_Get_Navigation_Properties()
        {
            var player = new Player();
            var robot = new Robot();

            var pr = new PlayersRobot
            {
                Player = player,
                Robot = robot
            };

            Assert.Same(player, pr.Player);
            Assert.Same(robot, pr.Robot);
        }

        [Fact]
        public void PlayersRobot_Allows_Null_For_Nullable_Properties()
        {
            var pr = new PlayersRobot
            {
                PlayerId = null,
                RobotId = null,
                Player = null,
                Robot = null
            };

            Assert.Null(pr.PlayerId);
            Assert.Null(pr.RobotId);
            Assert.Null(pr.Player);
            Assert.Null(pr.Robot);
        }
    }
}
