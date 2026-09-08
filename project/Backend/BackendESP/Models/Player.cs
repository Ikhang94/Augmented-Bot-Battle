using System;
using System.Collections.Generic;

namespace BackendESP.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Gamesession> GamesessionClientPlayers { get; set; } = new List<Gamesession>();

    public virtual ICollection<Gamesession> GamesessionHostPlayers { get; set; } = new List<Gamesession>();

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<PlayersRobot> PlayersRobots { get; set; } = new List<PlayersRobot>();
}
