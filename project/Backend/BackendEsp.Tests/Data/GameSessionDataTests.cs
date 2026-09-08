using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace BackendESP.Tests.Data;

public class GameSessionDataTests
{
    private PostgresDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PostgresDbContext(options);
    }

    // ──────────────── GetAllGameSessionsAsync ────────────────

    [Fact]
    public async Task GetAllGameSessionsAsync_WithSessions_Returns200()
    {
        using var context = CreateContext(nameof(GetAllGameSessionsAsync_WithSessions_Returns200));
        context.Gamesessions.AddRange(
            new Gamesession { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 },
            new Gamesession { Id = 2, HostPlayerId = 3, ClientPlayerId = 4, HostRobotId = 3, ClientRobotId = 4 }
        );
        await context.SaveChangesAsync();
        var sut = new GameSessionData(context);

        var result = await sut.GetAllGameSessionsAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetAllGameSessionsAsync_NoSessions_Returns200()
    {
        using var context = CreateContext(nameof(GetAllGameSessionsAsync_NoSessions_Returns200));
        var sut = new GameSessionData(context);

        var result = await sut.GetAllGameSessionsAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetAllGameSessionsAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetAllGameSessionsAsync_DbThrows_Returns500));
        var sut = new GameSessionData(context);
        await context.DisposeAsync();

        var result = await sut.GetAllGameSessionsAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── GetGameSessionAsync ────────────────

    [Fact]
    public async Task GetGameSessionAsync_SessionExists_Returns200()
    {
        using var context = CreateContext(nameof(GetGameSessionAsync_SessionExists_Returns200));
        context.Gamesessions.Add(new Gamesession { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 });
        await context.SaveChangesAsync();
        var sut = new GameSessionData(context);

        var result = await sut.GetGameSessionAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetGameSessionAsync_SessionNotFound_Returns500()
    {
        using var context = CreateContext(nameof(GetGameSessionAsync_SessionNotFound_Returns500));
        var sut = new GameSessionData(context);

        var result = await sut.GetGameSessionAsync(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetGameSessionAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetGameSessionAsync_DbThrows_Returns500));
        var sut = new GameSessionData(context);
        await context.DisposeAsync();

        var result = await sut.GetGameSessionAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── CreateGameSessionAsync ────────────────

    [Fact]
    public async Task CreateGameSessionAsync_ValidSession_Returns200()
    {
        using var context = CreateContext(nameof(CreateGameSessionAsync_ValidSession_Returns200));
        var sut = new GameSessionData(context);
        var dto = new GameSessionDTO { HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 };

        var result = await sut.CreateGameSessionAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CreateGameSessionAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(CreateGameSessionAsync_DbThrows_Returns500));
        var sut = new GameSessionData(context);
        await context.DisposeAsync();
        var dto = new GameSessionDTO { HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 };

        var result = await sut.CreateGameSessionAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── UpdateGameSessionAsync ────────────────

    [Fact]
    public async Task UpdateGameSessionAsync_SessionExists_Returns200()
    {
        using var context = CreateContext(nameof(UpdateGameSessionAsync_SessionExists_Returns200));
        context.Gamesessions.Add(new Gamesession { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 });
        await context.SaveChangesAsync();
        var sut = new GameSessionData(context);
        var dto = new GameSessionDTO { Id = 1, HostPlayerId = 3, ClientPlayerId = 4, HostRobotId = 3, ClientRobotId = 4 };

        var result = await sut.UpdateGameSessionAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateGameSessionAsync_SessionNotFound_Returns404()
    {
        using var context = CreateContext(nameof(UpdateGameSessionAsync_SessionNotFound_Returns404));
        var sut = new GameSessionData(context);
        var dto = new GameSessionDTO { Id = 99, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 };

        var result = await sut.UpdateGameSessionAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task UpdateGameSessionAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(UpdateGameSessionAsync_DbThrows_Returns500));
        var sut = new GameSessionData(context);
        await context.DisposeAsync();
        var dto = new GameSessionDTO { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 };

        var result = await sut.UpdateGameSessionAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── DeleteGameSessionAsync ────────────────

    [Fact]
    public async Task DeleteGameSessionAsync_SessionExists_Returns200()
    {
        using var context = CreateContext(nameof(DeleteGameSessionAsync_SessionExists_Returns200));
        context.Gamesessions.Add(new Gamesession { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 });
        await context.SaveChangesAsync();
        var sut = new GameSessionData(context);

        var result = await sut.DeleteGameSessionAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteGameSessionAsync_SessionNotFound_Returns404()
    {
        using var context = CreateContext(nameof(DeleteGameSessionAsync_SessionNotFound_Returns404));
        var sut = new GameSessionData(context);

        var result = await sut.DeleteGameSessionAsync(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DeleteGameSessionAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(DeleteGameSessionAsync_DbThrows_Returns500));
        var sut = new GameSessionData(context);
        await context.DisposeAsync();

        var result = await sut.DeleteGameSessionAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── GetGameSessionsOfPlayer ────────────────

    [Fact]
    public async Task GetGameSessionsOfPlayer_WithSessions_Returns200()
    {
        using var context = CreateContext(nameof(GetGameSessionsOfPlayer_WithSessions_Returns200));
        context.Gamesessions.AddRange(
            new Gamesession { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 },
            new Gamesession { Id = 2, HostPlayerId = 1, ClientPlayerId = 3, HostRobotId = 1, ClientRobotId = 3 }
        );
        await context.SaveChangesAsync();
        var sut = new GameSessionData(context);

        var result = await sut.GetGameSessionsOfPlayer(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetGameSessionsOfPlayer_NoSessions_Returns200()
    {
        using var context = CreateContext(nameof(GetGameSessionsOfPlayer_NoSessions_Returns200));
        var sut = new GameSessionData(context);

        var result = await sut.GetGameSessionsOfPlayer(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetGameSessionsOfPlayer_AsHostPlayer_Returns200()
    {
        using var context = CreateContext(nameof(GetGameSessionsOfPlayer_AsHostPlayer_Returns200));
        context.Gamesessions.Add(new Gamesession { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 });
        await context.SaveChangesAsync();
        var sut = new GameSessionData(context);

        var result = await sut.GetGameSessionsOfPlayer(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetGameSessionsOfPlayer_AsClientPlayer_Returns200()
    {
        using var context = CreateContext(nameof(GetGameSessionsOfPlayer_AsClientPlayer_Returns200));
        context.Gamesessions.Add(new Gamesession { Id = 1, HostPlayerId = 1, ClientPlayerId = 2, HostRobotId = 1, ClientRobotId = 2 });
        await context.SaveChangesAsync();
        var sut = new GameSessionData(context);

        var result = await sut.GetGameSessionsOfPlayer(2);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetGameSessionsOfPlayer_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetGameSessionsOfPlayer_DbThrows_Returns500));
        var sut = new GameSessionData(context);
        await context.DisposeAsync();

        var result = await sut.GetGameSessionsOfPlayer(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }
}
