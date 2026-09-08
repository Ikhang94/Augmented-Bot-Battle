using BackendESP.DTO;
using BackendESP.Models;
using Xunit;
using FluentAssertions;

namespace BackendESP.Tests.Models;

public class MissingModelsTests
{
    [Fact]
    public void Leaderboard_Properties_AreSetCorrectly()
    {
        var player = new Player { PlayerId = 1, Username = "Alice" };
        var lb = new Leaderboard
        {
            LeaderboardId = 10,
            PlayerId      = 1,
            Rank          = 3,
            Player        = player
        };

        lb.LeaderboardId.Should().Be(10);
        lb.PlayerId.Should().Be(1);
        lb.Rank.Should().Be(3);
        lb.Player.Should().Be(player);
    }

    [Fact]
    public void PlayerDTO_Properties_AreSetCorrectly()
    {
        var dto = new PlayerDTO
        {
            PlayerID = 5,
            UserName = "TestUser",
            Email    = "test@test.com",
            Password = "secret"
        };

        dto.PlayerID.Should().Be(5);
        dto.UserName.Should().Be("TestUser");
        dto.Email.Should().Be("test@test.com");
        dto.Password.Should().Be("secret");
    }

    [Fact]
    public void RobotDTO_Properties_AreSetCorrectly()
    {
        var dto = new RobotDTO { RobotName = "WALL-E" };

        dto.RobotName.Should().Be("WALL-E");
    }

}
