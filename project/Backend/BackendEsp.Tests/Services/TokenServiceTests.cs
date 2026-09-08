using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Models;
using BackendESP.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;
using FluentAssertions;

namespace BackendESP.Tests.Services;

public class TokenServiceTests
{
    private IConfiguration CreateMockConfiguration()
    {
        var configValues = new Dictionary<string, string>
        {
            { "JwtSettings:Key", "this-is-a-test-secret-key-that-is-long-enough-for-256-bit-hmac" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" }
        };

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x[It.IsAny<string>()])
            .Returns<string>(key => configValues.ContainsKey(key) ? configValues[key] : null);

        return mockConfig.Object;
    }

    [Fact]
    public async Task GenerateToken_ValidPlayer_ReturnsJwtToken()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var playerDataMock = new Mock<IPlayerData>();
        var playerDTO = new PlayerDTO { Email = "test@test.com", UserName = "testuser" };
        var player = new Player { PlayerId = 1, Username = "testuser", Email = "test@test.com", Password = "pwd" };

        playerDataMock
            .Setup(x => x.GetPlayerAsyncByEmailToken(playerDTO.Email))
            .ReturnsAsync(new PlayerDTO {
                PlayerID = player.PlayerId,
                UserName = player.Username,
                Email = player.Email,
                Password = player.Password
            });

        var sut = new TokenService(config, playerDataMock.Object);

        // Act
        var token = await sut.GenerateToken(playerDTO);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        jwtToken.Should().NotBeNull();
        jwtToken!.Issuer.Should().Be("TestIssuer");
        jwtToken.Audiences.Should().Contain("TestAudience");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "test@test.com");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "testuser");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
    }

    [Fact]
    public async Task GenerateToken_TokenContainsCorrectClaims()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var playerDataMock = new Mock<IPlayerData>();
        var playerDTO = new PlayerDTO { Email = "alice@test.com", UserName = "alice" };
        var player = new Player { PlayerId = 42, Username = "alice", Email = "alice@test.com", Password = "pwd" };

        playerDataMock
            .Setup(x => x.GetPlayerAsyncByEmailToken(playerDTO.Email))
            .ReturnsAsync(new PlayerDTO {
                PlayerID = player.PlayerId,
                UserName = player.Username,
                Email = player.Email,
                Password = player.Password
            });

        var sut = new TokenService(config, playerDataMock.Object);

        // Act
        var token = await sut.GenerateToken(playerDTO);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        jwtToken!.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value.Should().Be("42");
        jwtToken.Claims.Single(c => c.Type == ClaimTypes.Email).Value.Should().Be("alice@test.com");
        jwtToken.Claims.Single(c => c.Type == ClaimTypes.Name).Value.Should().Be("alice");
    }

    [Fact]
    public async Task GenerateToken_TokenExpiresIn1Hour()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var playerDataMock = new Mock<IPlayerData>();
        var playerDTO = new PlayerDTO { Email = "test@test.com", UserName = "testuser" };
        var player = new Player { PlayerId = 1, Username = "testuser", Email = "test@test.com", Password = "pwd" };

        playerDataMock
            .Setup(x => x.GetPlayerAsyncByEmailToken(playerDTO.Email))
            .ReturnsAsync(new PlayerDTO {
                PlayerID = player.PlayerId,
                UserName = player.Username,
                Email = player.Email,
                Password = player.Password
            });

        var sut = new TokenService(config, playerDataMock.Object);
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = await sut.GenerateToken(playerDTO);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        var expiryTime = jwtToken!.ValidTo;

        var timeDiff = expiryTime - beforeGeneration;
        timeDiff.Should().BeCloseTo(TimeSpan.FromHours(1), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task GenerateToken_CallsPlayerDataLayer()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var playerDataMock = new Mock<IPlayerData>();
        var playerDTO = new PlayerDTO { Email = "test@test.com" };
        var player = new Player { PlayerId = 1, Username = "testuser", Email = "test@test.com", Password = "pwd" };

        playerDataMock
            .Setup(x => x.GetPlayerAsyncByEmailToken(playerDTO.Email))
            .ReturnsAsync(new PlayerDTO {
                PlayerID = player.PlayerId,
                UserName = player.Username,
                Email = player.Email,
                Password = player.Password
            });

        var sut = new TokenService(config, playerDataMock.Object);

        // Act
        await sut.GenerateToken(playerDTO);

        // Assert
        playerDataMock.Verify(x => x.GetPlayerAsyncByEmailToken("test@test.com"), Times.Once);
    }
}
