using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace BackendESP.Tests.Data;

public class RobotDataTests
{
    private PostgresDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new PostgresDbContext(options);
    }

    // ──────────────── GetAllRobotsAsync ────────────────

    [Fact]
    public async Task GetAllRobotsAsync_WithRobots_Returns200()
    {
        using var context = CreateContext(nameof(GetAllRobotsAsync_WithRobots_Returns200));
        context.Robots.AddRange(
            new Robot { RobotId = 1, Name = "R2D2" },
            new Robot { RobotId = 2, Name = "C3PO" }
        );
        await context.SaveChangesAsync();
        var sut = new RobotData(context);

        var result = await sut.GetAllRobotsAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetAllRobotsAsync_NoRobots_Returns200()
    {
        using var context = CreateContext(nameof(GetAllRobotsAsync_NoRobots_Returns200));
        var sut = new RobotData(context);

        var result = await sut.GetAllRobotsAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    // ──────────────── GetRobotAsync ────────────────

    [Fact]
    public async Task GetRobotAsync_RobotExists_Returns200()
    {
        using var context = CreateContext(nameof(GetRobotAsync_RobotExists_Returns200));
        context.Robots.Add(new Robot { RobotId = 1, Name = "R2D2" });
        await context.SaveChangesAsync();
        var sut = new RobotData(context);

        var result = await sut.GetRobotAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetRobotAsync_RobotNotFound_Returns404()
    {
        using var context = CreateContext(nameof(GetRobotAsync_RobotNotFound_Returns404));
        var sut = new RobotData(context);

        var result = await sut.GetRobotAsync(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    // ──────────────── CreateRobotAsync ────────────────

    [Fact]
    public async Task CreateRobotAsync_ValidRobot_Returns200()
    {
        using var context = CreateContext(nameof(CreateRobotAsync_ValidRobot_Returns200));
        var sut = new RobotData(context);
        var dto = new RobotDTO { RobotName = "Wall-E" };

        var result = await sut.CreateRobotAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CreateRobotAsync_AlreadyExists_Returns409Or400()
    {
        using var context = CreateContext(nameof(CreateRobotAsync_AlreadyExists_Returns409Or400));
        context.Robots.Add(new Robot { RobotId = 1, Name = "Wall-E" });
        await context.SaveChangesAsync();
        var sut = new RobotData(context);
        var dto = new RobotDTO { RobotName = "Wall-E" };

        var result = await sut.CreateRobotAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().BeOneOf(400, 409);
    }

    // ──────────────── UpdateRobotAsync ────────────────

    [Fact]
    public async Task UpdateRobotAsync_RobotExists_Returns200()
    {
        using var context = CreateContext(nameof(UpdateRobotAsync_RobotExists_Returns200));
        context.Robots.Add(new Robot { RobotId = 1, Name = "R2D2" });
        await context.SaveChangesAsync();
        var sut    = new RobotData(context);
        var oldDto = new RobotDTO { RobotName = "R2D2" };
        var newDto = new RobotDTO { RobotName = "R2D2-Updated" };

        var result = await sut.UpdateRobotAsync(oldDto, newDto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateRobotAsync_RobotNotFound_Returns404()
    {
        using var context = CreateContext(nameof(UpdateRobotAsync_RobotNotFound_Returns404));
        var sut    = new RobotData(context);
        var oldDto = new RobotDTO { RobotName = "Ghost" };
        var newDto = new RobotDTO { RobotName = "GhostX" };

        var result = await sut.UpdateRobotAsync(oldDto, newDto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    // ──────────────── DeleteRobotAsync ────────────────

    [Fact]
    public async Task DeleteRobotAsync_RobotExists_Returns200()
    {
        using var context = CreateContext(nameof(DeleteRobotAsync_RobotExists_Returns200));
        context.Robots.Add(new Robot { RobotId = 1, Name = "R2D2" });
        await context.SaveChangesAsync();
        var sut = new RobotData(context);

        var result = await sut.DeleteRobotAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteRobotAsync_RobotNotFound_Returns404()
    {
        using var context = CreateContext(nameof(DeleteRobotAsync_RobotNotFound_Returns404));
        var sut = new RobotData(context);

        var result = await sut.DeleteRobotAsync(99);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(404);
    }

    // ──────────────── Tests des catch blocks (500) ────────────────

    [Fact]
    public async Task GetAllRobotsAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetAllRobotsAsync_DbThrows_Returns500));
        var sut = new RobotData(context);
        await context.DisposeAsync();

        var result = await sut.GetAllRobotsAsync();

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetRobotAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(GetRobotAsync_DbThrows_Returns500));
        var sut = new RobotData(context);
        await context.DisposeAsync();

        var result = await sut.GetRobotAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateRobotAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(CreateRobotAsync_DbThrows_Returns500));
        var sut = new RobotData(context);
        await context.DisposeAsync();
        var dto = new RobotDTO { RobotName = "Wall-E" };

        var result = await sut.CreateRobotAsync(dto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateRobotAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(UpdateRobotAsync_DbThrows_Returns500));
        var sut = new RobotData(context);
        await context.DisposeAsync();
        var oldDto = new RobotDTO { RobotName = "R2D2" };
        var newDto = new RobotDTO { RobotName = "R2D2-Updated" };

        var result = await sut.UpdateRobotAsync(oldDto, newDto);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task DeleteRobotAsync_DbThrows_Returns500()
    {
        var context = CreateContext(nameof(DeleteRobotAsync_DbThrows_Returns500));
        var sut = new RobotData(context);
        await context.DisposeAsync();

        var result = await sut.DeleteRobotAsync(1);

        (result as IStatusCodeHttpResult)!.StatusCode.Should().Be(500);
    }

}
