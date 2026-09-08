using System;
using System.Linq;
using BackendESP.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BackendESP.Tests.Models
{
    public class PostgresDbContextTests
    {
        private static PostgresDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new PostgresDbContext(options);
        }

        [Fact]
        public void DbSets_Are_Available()
        {
            using var ctx = CreateContext();

            Assert.NotNull(ctx.Gamesessions);
            Assert.NotNull(ctx.Leaderboards);
            Assert.NotNull(ctx.Players);
            Assert.NotNull(ctx.PlayersRobots);
            Assert.NotNull(ctx.Robots);
        }

        [Fact]
        public void Model_Maps_Tables_And_Columns_Player()
        {
            using var ctx = CreateContext();

            var entity = ctx.Model.FindEntityType(typeof(Player));
            Assert.NotNull(entity);

            Assert.Equal("players", entity!.GetTableName());

            var playerId = entity.FindProperty(nameof(Player.PlayerId));
            Assert.Equal("player_id", playerId!.GetColumnName());

            var username = entity.FindProperty(nameof(Player.Username));
            Assert.Equal("username", username!.GetColumnName());
            Assert.Equal(100, username!.GetMaxLength());

            var email = entity.FindProperty(nameof(Player.Email));
            Assert.Equal("email", email!.GetColumnName());
            Assert.Equal(100, email!.GetMaxLength());

            var password = entity.FindProperty(nameof(Player.Password));
            Assert.Equal("password", password!.GetColumnName());
            Assert.Equal(100, password!.GetMaxLength());
        }

        [Fact]
        public void Model_Maps_Tables_And_Columns_Robot()
        {
            using var ctx = CreateContext();

            var entity = ctx.Model.FindEntityType(typeof(Robot));
            Assert.NotNull(entity);

            Assert.Equal("robots", entity!.GetTableName());

            var robotId = entity.FindProperty(nameof(Robot.RobotId));
            Assert.Equal("robot_id", robotId!.GetColumnName());

            var name = entity.FindProperty(nameof(Robot.Name));
            Assert.Equal("name", name!.GetColumnName());
            Assert.Equal(100, name!.GetMaxLength());
        }

        [Fact]
        public void Model_Maps_Tables_And_Columns_Leaderboard_And_FK()
        {
            using var ctx = CreateContext();

            var entity = ctx.Model.FindEntityType(typeof(Leaderboard));
            Assert.NotNull(entity);

            Assert.Equal("leaderboard", entity!.GetTableName());

            Assert.Equal("leaderboard_id",
                entity.FindProperty(nameof(Leaderboard.LeaderboardId))!.GetColumnName());
            Assert.Equal("player_id",
                entity.FindProperty(nameof(Leaderboard.PlayerId))!.GetColumnName());
            Assert.Equal("rank",
                entity.FindProperty(nameof(Leaderboard.Rank))!.GetColumnName());

            // FK Leaderboard.PlayerId -> Player.PlayerId
            var fk = entity.GetForeignKeys()
                .Single(f => f.Properties.Any(p => p.Name == nameof(Leaderboard.PlayerId)));

            Assert.Equal(typeof(Player), fk.PrincipalEntityType.ClrType);
            Assert.Equal(nameof(Player.PlayerId), fk.PrincipalKey.Properties.Single().Name);
        }

        [Fact]
        public void Model_Maps_PlayersRobots_Table_And_FKs()
        {
            using var ctx = CreateContext();

            var entity = ctx.Model.FindEntityType(typeof(PlayersRobot));
            Assert.NotNull(entity);

            Assert.Equal("players_robots", entity!.GetTableName());

            Assert.Equal("id", entity.FindProperty(nameof(PlayersRobot.Id))!.GetColumnName());
            Assert.Equal("player_id", entity.FindProperty(nameof(PlayersRobot.PlayerId))!.GetColumnName());
            Assert.Equal("robot_id", entity.FindProperty(nameof(PlayersRobot.RobotId))!.GetColumnName());

            var fks = entity.GetForeignKeys().ToList();
            Assert.Equal(2, fks.Count);

            Assert.Contains(fks, fk =>
                fk.Properties.Single().Name == nameof(PlayersRobot.PlayerId) &&
                fk.PrincipalEntityType.ClrType == typeof(Player));

            Assert.Contains(fks, fk =>
                fk.Properties.Single().Name == nameof(PlayersRobot.RobotId) &&
                fk.PrincipalEntityType.ClrType == typeof(Robot));
        }

        [Fact]
        public void Model_Maps_Gamesessions_Table_Columns_And_FKs()
        {
            using var ctx = CreateContext();

            var entity = ctx.Model.FindEntityType(typeof(Gamesession));
            Assert.NotNull(entity);

            Assert.Equal("gamesessions", entity!.GetTableName());

            Assert.Equal("id", entity.FindProperty(nameof(Gamesession.Id))!.GetColumnName());
            Assert.Equal("host_player_id", entity.FindProperty(nameof(Gamesession.HostPlayerId))!.GetColumnName());
            Assert.Equal("client_player_id", entity.FindProperty(nameof(Gamesession.ClientPlayerId))!.GetColumnName());
            Assert.Equal("host_robot_id", entity.FindProperty(nameof(Gamesession.HostRobotId))!.GetColumnName());
            Assert.Equal("client_robot_id", entity.FindProperty(nameof(Gamesession.ClientRobotId))!.GetColumnName());
            Assert.Equal("start_time", entity.FindProperty(nameof(Gamesession.StartTime))!.GetColumnName());
            Assert.Equal("end_time", entity.FindProperty(nameof(Gamesession.EndTime))!.GetColumnName());

            // 4 FKs attendues: host/client player + host/client robot
            var fks = entity.GetForeignKeys().ToList();
            Assert.Equal(4, fks.Count);

            Assert.Contains(fks, fk =>
                fk.Properties.Single().Name == nameof(Gamesession.ClientPlayerId) &&
                fk.PrincipalEntityType.ClrType == typeof(Player));

            Assert.Contains(fks, fk =>
                fk.Properties.Single().Name == nameof(Gamesession.HostPlayerId) &&
                fk.PrincipalEntityType.ClrType == typeof(Player));

            Assert.Contains(fks, fk =>
                fk.Properties.Single().Name == nameof(Gamesession.ClientRobotId) &&
                fk.PrincipalEntityType.ClrType == typeof(Robot));

            Assert.Contains(fks, fk =>
                fk.Properties.Single().Name == nameof(Gamesession.HostRobotId) &&
                fk.PrincipalEntityType.ClrType == typeof(Robot));
        }
    }
}
