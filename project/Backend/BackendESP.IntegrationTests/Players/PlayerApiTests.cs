using System.Net;
using System.Net.Http.Json;
using BackendESP.DTO;
using BackendESP.IntegrationTests.Infrastructure;

namespace BackendESP.IntegrationTests.Players;

public class PlayerApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public PlayerApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // ─── POST /api/users ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreatePlayer_Returns200_WhenValidPlayerProvided()
    {
        var dto = new PlayerDTO
        {
            UserName = $"user_{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@test.com",
            Password = "password123",
        };

        var response = await _client.PostAsJsonAsync("/api/users", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatePlayer_Returns404_WhenUserNameIsNull()
    {
        var dto = new PlayerDTO
        {
            UserName = null!,
            Email = $"{Guid.NewGuid():N}@test.com",
            Password = "password123",
        };

        var response = await _client.PostAsJsonAsync("/api/users", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ─── GET /api/users/getall ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllPlayers_Returns200_WithSeededPlayer()
    {
        var email = $"{Guid.NewGuid():N}@test.com";
        await _factory.SeedPlayerAsync("alice", email, "pw");

        var response = await _client.GetAsync("/api/users/getall");
        var players = await response.Content.ReadFromJsonAsync<List<PlayerDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(players);
        Assert.Contains(players, p => p.Email == email);
    }

    // ─── GET /api/users/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task GetPlayer_Returns200_WhenPlayerExists()
    {
        var email = $"{Guid.NewGuid():N}@test.com";
        var id = await _factory.SeedPlayerAsync("bob", email, "pw");

        var response = await _client.GetAsync($"/api/users/{id}");
        var player = await response.Content.ReadFromJsonAsync<PlayerDTO>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(player);
        Assert.Equal(email, player.Email);
    }

    [Fact]
    public async Task GetPlayer_Returns404_WhenPlayerDoesNotExist()
    {
        var response = await _client.GetAsync("/api/users/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ─── PUT /api/users ───────────────────────────────────────────────────────

    [Fact]
    public async Task UpdatePlayer_Returns200_WhenPlayerExists()
    {
        var email = $"{Guid.NewGuid():N}@test.com";
        await _factory.SeedPlayerAsync("charlie", email, "oldpass");

        var oldDto = new PlayerDTO { UserName = "charlie", Email = email, Password = "oldpass" };
        var newDto = new PlayerDTO { UserName = "charlie_v2", Email = email, Password = "newpass" };

        var response = await _client.PutAsJsonAsync("/api/users", new[] { oldDto, newDto });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayer_Returns400_WhenPlayerNotFound()
    {
        var oldDto = new PlayerDTO { UserName = "ghost", Email = "ghost@nowhere.com", Password = "x" };
        var newDto = new PlayerDTO { UserName = "ghost2", Email = "ghost2@nowhere.com", Password = "y" };

        var response = await _client.PutAsJsonAsync("/api/users", new[] { oldDto, newDto });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ─── DELETE /api/users/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task DeletePlayer_Returns200_WhenPlayerExists()
    {
        var id = await _factory.SeedPlayerAsync("dave", $"{Guid.NewGuid():N}@test.com", "pw");

        var response = await _client.DeleteAsync($"/api/users/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeletePlayer_Returns404_WhenPlayerDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/users/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
