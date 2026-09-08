using System;
using BackendESP.Models;
using Xunit;

namespace BackendESP.Tests.Models
{
    public class GamesessionTests
    {
        [Fact]
        public void Gamesession_Can_Set_And_Get_Scalar_Properties()
        {
            var start = new DateTime(2026, 01, 15, 2, 00, 00, DateTimeKind.Utc);
            var end = start.AddMinutes(10);

            var session = new Gamesession
            {
                Id = 123,
                HostPlayerId = 1,
                ClientPlayerId = 2,
                HostRobotId = 10,
                ClientRobotId = 20,
                StartTime = start,
                EndTime = end
            };

            Assert.Equal(123, session.Id);
            Assert.Equal(1, session.HostPlayerId);
            Assert.Equal(2, session.ClientPlayerId);
            Assert.Equal(10, session.HostRobotId);
            Assert.Equal(20, session.ClientRobotId);
            Assert.Equal(start, session.StartTime);
            Assert.Equal(end, session.EndTime);
        }

        [Fact]
        public void Gamesession_Can_Set_And_Get_Navigation_Properties()
        {
            var hostPlayer = new Player();
            var clientPlayer = new Player();
            var hostRobot = new Robot();
            var clientRobot = new Robot();

            var session = new Gamesession
            {
                HostPlayer = hostPlayer,
                ClientPlayer = clientPlayer,
                HostRobot = hostRobot,
                ClientRobot = clientRobot
            };

            Assert.Same(hostPlayer, session.HostPlayer);
            Assert.Same(clientPlayer, session.ClientPlayer);
            Assert.Same(hostRobot, session.HostRobot);
            Assert.Same(clientRobot, session.ClientRobot);
        }

        [Fact]
        public void Gamesession_Allows_Null_For_Nullable_Properties()
        {
            var session = new Gamesession
            {
                HostPlayerId = null,
                ClientPlayerId = null,
                HostRobotId = null,
                ClientRobotId = null,
                StartTime = null,
                EndTime = null,
                HostPlayer = null,
                ClientPlayer = null,
                HostRobot = null,
                ClientRobot = null
            };

            Assert.Null(session.HostPlayerId);
            Assert.Null(session.ClientPlayerId);
            Assert.Null(session.HostRobotId);
            Assert.Null(session.ClientRobotId);
            Assert.Null(session.StartTime);
            Assert.Null(session.EndTime);
            Assert.Null(session.HostPlayer);
            Assert.Null(session.ClientPlayer);
            Assert.Null(session.HostRobot);
            Assert.Null(session.ClientRobot);
        }
    }
}
