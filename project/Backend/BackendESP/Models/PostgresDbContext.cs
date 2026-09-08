using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BackendESP.Models;

public partial class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Gamesession> Gamesessions { get; set; }

    public virtual DbSet<Leaderboard> Leaderboards { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayersRobot> PlayersRobots { get; set; }

    public virtual DbSet<Robot> Robots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gamesession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("gamesessions_pkey");

            entity.ToTable("gamesessions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientPlayerId).HasColumnName("client_player_id");
            entity.Property(e => e.ClientRobotId).HasColumnName("client_robot_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.HostPlayerId).HasColumnName("host_player_id");
            entity.Property(e => e.HostRobotId).HasColumnName("host_robot_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");

            entity.HasOne(d => d.ClientPlayer).WithMany(p => p.GamesessionClientPlayers)
                .HasForeignKey(d => d.ClientPlayerId)
                .HasConstraintName("gamesessions_client_player_id_fkey");

            entity.HasOne(d => d.ClientRobot).WithMany(p => p.GamesessionClientRobots)
                .HasForeignKey(d => d.ClientRobotId)
                .HasConstraintName("gamesessions_client_robot_id_fkey");

            entity.HasOne(d => d.HostPlayer).WithMany(p => p.GamesessionHostPlayers)
                .HasForeignKey(d => d.HostPlayerId)
                .HasConstraintName("gamesessions_host_player_id_fkey");

            entity.HasOne(d => d.HostRobot).WithMany(p => p.GamesessionHostRobots)
                .HasForeignKey(d => d.HostRobotId)
                .HasConstraintName("gamesessions_host_robot_id_fkey");
        });

        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(e => e.LeaderboardId).HasName("leaderboard_pkey");

            entity.ToTable("leaderboard");

            entity.Property(e => e.LeaderboardId).HasColumnName("leaderboard_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Rank).HasColumnName("rank");

            entity.HasOne(d => d.Player).WithMany(p => p.Leaderboards)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("leaderboard_player_id_fkey");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("players_pkey");

            entity.ToTable("players");

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<PlayersRobot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("players_robots_pkey");

            entity.ToTable("players_robots");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.RobotId).HasColumnName("robot_id");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayersRobots)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("players_robots_player_id_fkey");

            entity.HasOne(d => d.Robot).WithMany(p => p.PlayersRobots)
                .HasForeignKey(d => d.RobotId)
                .HasConstraintName("players_robots_robot_id_fkey");
        });

        modelBuilder.Entity<Robot>(entity =>
        {
            entity.HasKey(e => e.RobotId).HasName("robots_pkey");

            entity.ToTable("robots");

            entity.Property(e => e.RobotId).HasColumnName("robot_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
