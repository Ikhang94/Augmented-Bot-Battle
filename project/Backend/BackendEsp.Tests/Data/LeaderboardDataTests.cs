using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace BackendESP.Tests.Data;

public class LeaderboardDataTests
{
    private PostgresDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PostgresDbContext(options);
    }

    // ──────────────── GetLeaderboard ────────────────

    [Fact]
    public async Task GetLeaderboard_WithEntries_Returns200()
    {
        using var context = CreateContext(nameof(GetLeaderboard_WithEntries_Returns200));
        context.Leaderboards.AddRange(
            new Leaderboard { LeaderboardId = 1, PlayerId = 1, Rank = 1 },
            new Leaderboard { LeaderboardId = 2, PlayerId = 2, Rank = 2 }
        );
        await context.SaveChangesAsync();
        var sut = new LeaderboardData(context);

        var result = await sut.GetLeaderboard();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetLeaderboard_Empty_Returns200()
    {
        using var context = CreateContext(nameof(GetLeaderboard_Empty_Returns200));
        var sut = new LeaderboardData(context);

        var result = await sut.GetLeaderboard();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetLeaderboard_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetLeaderboard_DbThrows_Returns500));
        var sut = new LeaderboardData(context);
        await context.DisposeAsync();

        var result = await sut.GetLeaderboard();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── CreateLeaderboard ────────────────

    [Fact]
    public async Task CreateLeaderboard_NewPlayer_Returns200()
    {
        using var context = CreateContext(nameof(CreateLeaderboard_NewPlayer_Returns200));
        var sut = new LeaderboardData(context);

        var result = await sut.CreateLeaderboard(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CreateLeaderboard_PlayerAlreadyExists_Returns400()
    {
        using var context = CreateContext(nameof(CreateLeaderboard_PlayerAlreadyExists_Returns400));
        context.Leaderboards.Add(new Leaderboard { LeaderboardId = 1, PlayerId = 1, Rank = 1 });
        await context.SaveChangesAsync();
        var sut = new LeaderboardData(context);

        var result = await sut.CreateLeaderboard(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateLeaderboard_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(CreateLeaderboard_DbThrows_Returns500));
        var sut = new LeaderboardData(context);
        await context.DisposeAsync();

        var result = await sut.CreateLeaderboard(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    // ──────────────── SwapLeaderboard ────────────────

    [Fact]
    public async Task SwapLeaderboard_BothPlayersExist_Returns200()
    {
        using var context = CreateContext(nameof(SwapLeaderboard_BothPlayersExist_Returns200));
        context.Leaderboards.AddRange(
            new Leaderboard { LeaderboardId = 1, PlayerId = 1, Rank = 1 },
            new Leaderboard { LeaderboardId = 2, PlayerId = 2, Rank = 2 }
        );
        await context.SaveChangesAsync();
        var sut = new LeaderboardData(context);
        var player1 = new LeaderboardDTO { PlayerId = 1, Rank = 1 };
        var player2 = new LeaderboardDTO { PlayerId = 2, Rank = 2 };

        var result = await sut.SwapLeaderboard(player1, player2);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task SwapLeaderboard_Player1NotFound_Returns400()
    {
        using var context = CreateContext(nameof(SwapLeaderboard_Player1NotFound_Returns400));
        context.Leaderboards.Add(new Leaderboard { LeaderboardId = 2, PlayerId = 2, Rank = 2 });
        await context.SaveChangesAsync();
        var sut = new LeaderboardData(context);
        var player1 = new LeaderboardDTO { PlayerId = 99, Rank = 1 };
        var player2 = new LeaderboardDTO { PlayerId = 2, Rank = 2 };

        var result = await sut.SwapLeaderboard(player1, player2);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SwapLeaderboard_Player2NotFound_Returns400()
    {
        using var context = CreateContext(nameof(SwapLeaderboard_Player2NotFound_Returns400));
        context.Leaderboards.Add(new Leaderboard { LeaderboardId = 1, PlayerId = 1, Rank = 1 });
        await context.SaveChangesAsync();
        var sut = new LeaderboardData(context);
        var player1 = new LeaderboardDTO { PlayerId = 1, Rank = 1 };
        var player2 = new LeaderboardDTO { PlayerId = 99, Rank = 2 };

        var result = await sut.SwapLeaderboard(player1, player2);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SwapLeaderboard_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(SwapLeaderboard_DbThrows_Returns500));
        var sut = new LeaderboardData(context);
        await context.DisposeAsync();
        var player1 = new LeaderboardDTO { PlayerId = 1, Rank = 1 };
        var player2 = new LeaderboardDTO { PlayerId = 2, Rank = 2 };

        var result = await sut.SwapLeaderboard(player1, player2);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }
}
