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
    public class RobotControllerTests
    {
        private static RobotController CreateController(Mock<IRobotData> robotDataMock)
            => new RobotController(robotDataMock.Object);

        [Fact]
        public async Task GetAllRobots_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            IResult expected = Results.Ok();

            var robotDataMock = new Mock<IRobotData>(MockBehavior.Strict);
            robotDataMock
                .Setup(x => x.GetAllRobotsAsync())
                .ReturnsAsync(expected);

            var controller = CreateController(robotDataMock);

            // Act
            var result = await controller.GetAllRobots();

            // Assert
            Assert.Same(expected, result);
            robotDataMock.Verify(x => x.GetAllRobotsAsync(), Times.Once);
            robotDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateRobot_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            var robot = new RobotDTO();
            IResult expected = Results.Ok();

            var robotDataMock = new Mock<IRobotData>(MockBehavior.Strict);
            robotDataMock
                .Setup(x => x.CreateRobotAsync(robot))
                .ReturnsAsync(expected);

            var controller = CreateController(robotDataMock);

            // Act
            var result = await controller.CreateRobot(robot);

            // Assert
            Assert.Same(expected, result);
            robotDataMock.Verify(x => x.CreateRobotAsync(robot), Times.Once);
            robotDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetRobotAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            var robotId = 42;
            IResult expected = Results.Ok();

            var robotDataMock = new Mock<IRobotData>(MockBehavior.Strict);
            robotDataMock
                .Setup(x => x.GetRobotAsync(robotId))
                .ReturnsAsync(expected);

            var controller = CreateController(robotDataMock);

            // Act
            var result = await controller.GetRobotAsync(robotId);

            // Assert
            Assert.Same(expected, result);
            robotDataMock.Verify(x => x.GetRobotAsync(robotId), Times.Once);
            robotDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateRobotAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            var oldRobot = new RobotDTO { RobotName = "OldRobot" };
            var newRobot = new RobotDTO { RobotName = "NewRobot" };
            var robots = new[] { oldRobot, newRobot };
            IResult expected = Results.Ok();

            var robotDataMock = new Mock<IRobotData>(MockBehavior.Strict);
            robotDataMock
                .Setup(x => x.UpdateRobotAsync(oldRobot, newRobot))
                .ReturnsAsync(expected);

            var controller = CreateController(robotDataMock);

            // Act
            var result = await controller.UpdateRobotAsync(robots);

            // Assert
            Assert.Same(expected, result);
            robotDataMock.Verify(x => x.UpdateRobotAsync(oldRobot, newRobot), Times.Once);
            robotDataMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteRobotAsync_Delegates_To_DataLayer_And_Returns_IResult()
        {
            // Arrange
            var robotId = 7;
            IResult expected = Results.Ok();

            var robotDataMock = new Mock<IRobotData>(MockBehavior.Strict);
            robotDataMock
                .Setup(x => x.DeleteRobotAsync(robotId))
                .ReturnsAsync(expected);

            var controller = CreateController(robotDataMock);

            // Act
            var result = await controller.DeleteRobotAsync(robotId);

            // Assert
            Assert.Same(expected, result);
            robotDataMock.Verify(x => x.DeleteRobotAsync(robotId), Times.Once);
            robotDataMock.VerifyNoOtherCalls();
        }
    }
}
