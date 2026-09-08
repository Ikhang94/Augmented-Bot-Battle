using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace BackendESP.Tests.Data;

public class PlayerDataTests
{
    private PostgresDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PostgresDbContext(options);
    }

    // ──────────────── GetAllPlayersAsync ────────────────

    [Fact]
    public async Task GetAllPlayersAsync_WithPlayers_Returns200()
    {
        using var context = CreateContext(nameof(GetAllPlayersAsync_WithPlayers_Returns200));
        context.Players.AddRange(
            new Player { PlayerId = 1, Username = "Alice", Email = "alice@test.com", Password = "p1" },
            new Player { PlayerId = 2, Username = "Bob",   Email = "bob@test.com",   Password = "p2" }
        );
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);

        var result = await sut.GetAllPlayersAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetAllPlayersAsync_NoPlayers_Returns200()
    {
        using var context = CreateContext(nameof(GetAllPlayersAsync_NoPlayers_Returns200));
        var sut = new PlayerData(context);

        var result = await sut.GetAllPlayersAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    // ──────────────── GetPlayerAsync ────────────────

    [Fact]
    public async Task GetPlayerAsync_PlayerExists_Returns200()
    {
        using var context = CreateContext(nameof(GetPlayerAsync_PlayerExists_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Alice", Email = "alice@test.com", Password = "p1" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetPlayerAsync_PlayerNotFound_Returns404()
    {
        using var context = CreateContext(nameof(GetPlayerAsync_PlayerNotFound_Returns404));
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerAsync(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    // ──────────────── CreatePlayerAsync ────────────────

    [Fact]
    public async Task CreatePlayerAsync_ValidPlayer_Returns200()
    {
        using var context = CreateContext(nameof(CreatePlayerAsync_ValidPlayer_Returns200));
        var sut = new PlayerData(context);
        var dto = new PlayerDTO { PlayerID = 0, UserName = "NewUser", Email = "new@test.com", Password = "pwd" };

        var result = await sut.CreatePlayerAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CreatePlayerAsync_AlreadyExists_Returns200()
    {
        using var context = CreateContext(nameof(CreatePlayerAsync_AlreadyExists_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Alice", Email = "alice@test.com", Password = "p1" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);
        var dto = new PlayerDTO { PlayerID = 1, UserName = "Alice", Email = "alice@test.com", Password = "p1" };

        var result = await sut.CreatePlayerAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    // ──────────────── UpdatePlayerAsync ────────────────

    [Fact]
    public async Task UpdatePlayerAsync_PlayerExists_Returns200()
    {
        using var context = CreateContext(nameof(UpdatePlayerAsync_PlayerExists_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Alice", Email = "alice@test.com", Password = "p1" });
        await context.SaveChangesAsync();
        var sut    = new PlayerData(context);
        var oldDto = new PlayerDTO { PlayerID = 1, UserName = "Alice",        Email = "alice@test.com",   Password = "p1"  };
        var newDto = new PlayerDTO { PlayerID = 1, UserName = "AliceUpdated", Email = "alice2@test.com",  Password = "p2"  };

        var result = await sut.UpdatePlayerAsync(oldDto, newDto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdatePlayerAsync_PlayerNotFound_Returns400()
    {
        using var context = CreateContext(nameof(UpdatePlayerAsync_PlayerNotFound_Returns400));
        var sut    = new PlayerData(context);
        var oldDto = new PlayerDTO { PlayerID = 99, UserName = "Ghost", Email = "ghost@test.com", Password = "p" };
        var newDto = new PlayerDTO { PlayerID = 99, UserName = "GhostX",Email = "ghost2@test.com", Password = "p2" };

        var result = await sut.UpdatePlayerAsync(oldDto, newDto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(400);
    }

    // ──────────────── DeletePlayerAsync ────────────────

    [Fact]
    public async Task DeletePlayerAsync_PlayerExists_Returns200()
    {
        using var context = CreateContext(nameof(DeletePlayerAsync_PlayerExists_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Alice", Email = "alice@test.com", Password = "p1" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);

        var result = await sut.DeletePlayerAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeletePlayerAsync_PlayerNotFound_Returns404()
    {
        using var context = CreateContext(nameof(DeletePlayerAsync_PlayerNotFound_Returns404));
        var sut = new PlayerData(context);

        var result = await sut.DeletePlayerAsync(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    // ──────────────── Tests des chemins manquants ────────────────

    // CreatePlayerAsync : validation null → NotFound (lignes 72-73)
    [Fact]
    public async Task CreatePlayerAsync_NullUsername_Returns404()
    {
        using var context = CreateContext(nameof(CreatePlayerAsync_NullUsername_Returns404));
        var sut = new PlayerData(context);
        var dto = new PlayerDTO { UserName = null, Email = "test@test.com", Password = "pwd" };

        var result = await sut.CreatePlayerAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreatePlayerAsync_NullEmailAndPassword_Returns404()
    {
        using var context = CreateContext(nameof(CreatePlayerAsync_NullEmailAndPassword_Returns404));
        var sut = new PlayerData(context);
        var dto = new PlayerDTO { UserName = "User", Email = null, Password = null };

        var result = await sut.CreatePlayerAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    // GetAllPlayersAsync : catch block → 500 (lignes 35-38)
    [Fact]
    public async Task GetAllPlayersAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetAllPlayersAsync_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync(); // force ObjectDisposedException dans le try

        var result = await sut.GetAllPlayersAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // GetPlayerAsync : catch block → 500 (lignes 64-66)
    [Fact]
    public async Task GetPlayerAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetPlayerAsync_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync();

        var result = await sut.GetPlayerAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // CreatePlayerAsync : catch block → 500 (lignes 87-88)
    [Fact]
    public async Task CreatePlayerAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(CreatePlayerAsync_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync();
        var dto = new PlayerDTO { UserName = "User", Email = "u@test.com", Password = "pwd" };

        var result = await sut.CreatePlayerAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // UpdatePlayerAsync : catch block → 500 (lignes 109-111)
    [Fact]
    public async Task UpdatePlayerAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(UpdatePlayerAsync_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync();
        var oldDto = new PlayerDTO { UserName = "Alice", Email = "alice@test.com", Password = "p1" };
        var newDto = new PlayerDTO { UserName = "AliceX", Email = "alice2@test.com", Password = "p2" };

        var result = await sut.UpdatePlayerAsync(oldDto, newDto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // DeletePlayerAsync : catch block → 500 (lignes 128-130)
    [Fact]
    public async Task DeletePlayerAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(DeletePlayerAsync_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync();

        var result = await sut.DeletePlayerAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── GetPlayerAsyncByEmail ────────────────

    [Fact]
    public async Task GetPlayerAsyncByEmail_PlayerExists_Returns200()
    {
        using var context = CreateContext(nameof(GetPlayerAsyncByEmail_PlayerExists_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Alice", Email = "alice@test.com", Password = "p1" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerAsyncByEmail("alice@test.com");

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetPlayerAsyncByEmail_PlayerNotFound_Returns404()
    {
        using var context = CreateContext(nameof(GetPlayerAsyncByEmail_PlayerNotFound_Returns404));
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerAsyncByEmail("notfound@test.com");

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetPlayerAsyncByEmail_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetPlayerAsyncByEmail_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync();

        var result = await sut.GetPlayerAsyncByEmail("test@test.com");

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── GetPlayerAsyncByEmailToken ────────────────

    [Fact]
    public async Task GetPlayerAsyncByEmailToken_PlayerExists_ReturnsPlayerDTO()
    {
        using var context = CreateContext(nameof(GetPlayerAsyncByEmailToken_PlayerExists_ReturnsPlayerDTO));
        context.Players.Add(new Player { PlayerId = 1, Username = "Bob", Email = "bob@test.com", Password = "p2" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerAsyncByEmailToken("bob@test.com");

        result.Should().NotBeNull();
        result!.UserName.Should().Be("Bob");
        result.Email.Should().Be("bob@test.com");
    }

    [Fact]
    public async Task GetPlayerAsyncByEmailToken_PlayerNotFound_ReturnsNull()
    {
        using var context = CreateContext(nameof(GetPlayerAsyncByEmailToken_PlayerNotFound_ReturnsNull));
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerAsyncByEmailToken("notfound@test.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPlayerAsyncByEmailToken_DbThrows_ReturnsNull()
    {
        var context = CreateContext(nameof(GetPlayerAsyncByEmailToken_DbThrows_ReturnsNull));
        var sut = new PlayerData(context);
        await context.DisposeAsync();

        var result = await sut.GetPlayerAsyncByEmailToken("test@test.com");

        result.Should().BeNull();
    }

    // ──────────────── PlayerLogin ────────────────

    [Fact]
    public async Task PlayerLogin_ValidCredentials_Returns200()
    {
        using var context = CreateContext(nameof(PlayerLogin_ValidCredentials_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Charlie", Email = "charlie@test.com", Password = BCrypt.Net.BCrypt.HashPassword("pwd123") });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);
        var loginDTO = new PlayerDTO { Email = "charlie@test.com", Password = "pwd123" };

        var result = await sut.PlayerLogin(loginDTO);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task PlayerLogin_InvalidPassword_Returns401()
    {
        using var context = CreateContext(nameof(PlayerLogin_InvalidPassword_Returns401));
        context.Players.Add(new Player { PlayerId = 1, Username = "David", Email = "david@test.com", Password = BCrypt.Net.BCrypt.HashPassword("pwd123") });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);
        var loginDTO = new PlayerDTO { Email = "david@test.com", Password = "wrongpwd" };

        var result = await sut.PlayerLogin(loginDTO);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task PlayerLogin_UserNotFound_Returns401()
    {
        using var context = CreateContext(nameof(PlayerLogin_UserNotFound_Returns401));
        var sut = new PlayerData(context);
        var loginDTO = new PlayerDTO { Email = "notfound@test.com", Password = "pwd" };

        var result = await sut.PlayerLogin(loginDTO);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(401);
    }

    // ──────────────── PlayerAcquireRobotAsync ────────────────

    [Fact]
    public async Task PlayerAcquireRobotAsync_ValidPlayerAndRobot_Returns200()
    {
        using var context = CreateContext(nameof(PlayerAcquireRobotAsync_ValidPlayerAndRobot_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Eve", Email = "eve@test.com", Password = "pwd" });
        context.Robots.Add(new Robot { RobotId = 1, Name = "RoboEve" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);
        var dto = new PlayersRobotDTO { PlayerId = 1, RobotId = 1 };

        var result = await sut.PlayerAcquireRobotAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task PlayerAcquireRobotAsync_PlayerNotFound_Returns404()
    {
        using var context = CreateContext(nameof(PlayerAcquireRobotAsync_PlayerNotFound_Returns404));
        context.Robots.Add(new Robot { RobotId = 1, Name = "Robot" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);
        var dto = new PlayersRobotDTO { PlayerId = 99, RobotId = 1 };

        var result = await sut.PlayerAcquireRobotAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task PlayerAcquireRobotAsync_RobotNotFound_Returns404()
    {
        using var context = CreateContext(nameof(PlayerAcquireRobotAsync_RobotNotFound_Returns404));
        context.Players.Add(new Player { PlayerId = 1, Username = "Frank", Email = "frank@test.com", Password = "pwd" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);
        var dto = new PlayersRobotDTO { PlayerId = 1, RobotId = 99 };

        var result = await sut.PlayerAcquireRobotAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task PlayerAcquireRobotAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(PlayerAcquireRobotAsync_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync();
        var dto = new PlayersRobotDTO { PlayerId = 1, RobotId = 1 };

        var result = await sut.PlayerAcquireRobotAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── GetPlayerRobotsAsync ────────────────

    [Fact]
    public async Task GetPlayerRobotsAsync_PlayerWithRobots_Returns200()
    {
        using var context = CreateContext(nameof(GetPlayerRobotsAsync_PlayerWithRobots_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Grace", Email = "grace@test.com", Password = "pwd" });
        context.Robots.Add(new Robot { RobotId = 1, Name = "Robot1" });
        context.PlayersRobots.Add(new PlayersRobot { PlayerId = 1, RobotId = 1 });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerRobotsAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetPlayerRobotsAsync_PlayerWithoutRobots_Returns200()
    {
        using var context = CreateContext(nameof(GetPlayerRobotsAsync_PlayerWithoutRobots_Returns200));
        context.Players.Add(new Player { PlayerId = 1, Username = "Henry", Email = "henry@test.com", Password = "pwd" });
        await context.SaveChangesAsync();
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerRobotsAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetPlayerRobotsAsync_PlayerNotFound_Returns404()
    {
        using var context = CreateContext(nameof(GetPlayerRobotsAsync_PlayerNotFound_Returns404));
        var sut = new PlayerData(context);

        var result = await sut.GetPlayerRobotsAsync(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetPlayerRobotsAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetPlayerRobotsAsync_DbThrows_Returns500));
        var sut = new PlayerData(context);
        await context.DisposeAsync();

        var result = await sut.GetPlayerRobotsAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

}

