using BackendESP.Controllers;
using BackendESP.Data;
using BackendESP.DTO;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using FluentAssertions;

namespace BackendESP.Tests.Controllers;

public class RobotControllerMissingTests
{
    [Fact]
    public async Task UpdateRobot_CallsUpdateRobotAsync_AndReturnsResult()
    {
        IResult expected = Results.Ok();
        var robots = new List<RobotDTO>
        {
            new RobotDTO { RobotName = "R2D2" },         // oldRobot
            new RobotDTO { RobotName = "R2D2-Updated" }  // newRobot
        };

        var mockData = new Mock<IRobotData>();
        mockData.Setup(d => d.UpdateRobotAsync(It.IsAny<RobotDTO>(), It.IsAny<RobotDTO>()))
                .ReturnsAsync(expected);
        var controller = new RobotController(mockData.Object);

        var result = await controller.UpdateRobotAsync(robots); // IEnumerable<RobotDTO>

        result.Should().NotBeNull();
        mockData.Verify(d => d.UpdateRobotAsync(It.IsAny<RobotDTO>(), It.IsAny<RobotDTO>()), Times.Once);
    }


    [Fact]
    public async Task DeleteRobot_CallsDeleteRobotAsync_AndReturnsResult()
    {
        IResult expected  = Results.Ok();
        var mockData      = new Mock<IRobotData>();
        mockData.Setup(d => d.DeleteRobotAsync(1)).ReturnsAsync(expected);
        var controller    = new RobotController(mockData.Object);

        var result = await controller.DeleteRobotAsync(1);

        mockData.Verify(d => d.DeleteRobotAsync(1), Times.Once);
        result.Should().NotBeNull();
    }
}
