using System.Net;
using System.Net.Http.Json;
using BackendESP.DTO;
using BackendESP.IntegrationTests.Infrastructure;

namespace BackendESP.IntegrationTests.Robots;

public class RobotApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RobotApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // ─── GET /api/robots ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllRobots_Returns200_WithSeededRobot()
    {
        var name = $"Robot_{Guid.NewGuid():N}";
        await _factory.SeedRobotAsync(name);

        var response = await _client.GetAsync("/api/robots");
        var robots = await response.Content.ReadFromJsonAsync<List<RobotDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(robots);
        Assert.Contains(robots, r => r.RobotName == name);
    }

    // ─── POST /api/robots ─────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRobot_Returns200_WhenNewRobot()
    {
        var dto = new RobotDTO { RobotName = $"Robot_{Guid.NewGuid():N}" };

        var response = await _client.PostAsJsonAsync("/api/robots", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateRobot_Returns400_WhenRobotAlreadyExists()
    {
        var name = $"Robot_{Guid.NewGuid():N}";
        await _factory.SeedRobotAsync(name);

        var dto = new RobotDTO { RobotName = name };
        var response = await _client.PostAsJsonAsync("/api/robots", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ─── GET /api/robots/{id} ─────────────────────────────────────────────────

    [Fact]
    public async Task GetRobot_Returns200_WhenRobotExists()
    {
        var name = $"Robot_{Guid.NewGuid():N}";
        var id = await _factory.SeedRobotAsync(name);

        var response = await _client.GetAsync($"/api/robots/{id}");
        var robot = await response.Content.ReadFromJsonAsync<RobotDTO>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(robot);
        Assert.Equal(name, robot.RobotName);
    }

    [Fact]
    public async Task GetRobot_Returns404_WhenRobotDoesNotExist()
    {
        var response = await _client.GetAsync("/api/robots/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ─── PUT /api/robots ──────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateRobot_Returns200_WhenRobotExists()
    {
        var oldName = $"Robot_{Guid.NewGuid():N}";
        await _factory.SeedRobotAsync(oldName);

        var oldDto = new RobotDTO { RobotName = oldName };
        var newDto = new RobotDTO { RobotName = $"Robot_{Guid.NewGuid():N}" };

        var response = await _client.PutAsJsonAsync("/api/robots", new[] { oldDto, newDto });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRobot_Returns404_WhenRobotNotFound()
    {
        var oldDto = new RobotDTO { RobotName = "GhostBot_DoesNotExist" };
        var newDto = new RobotDTO { RobotName = "GhostBot_v2" };

        var response = await _client.PutAsJsonAsync("/api/robots", new[] { oldDto, newDto });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ─── DELETE /api/robots/{id} ──────────────────────────────────────────────

    [Fact]
    public async Task DeleteRobot_Returns200_WhenRobotExists()
    {
        var id = await _factory.SeedRobotAsync($"Robot_{Guid.NewGuid():N}");

        var response = await _client.DeleteAsync($"/api/robots/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRobot_Returns404_WhenRobotDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/robots/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
