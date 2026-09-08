using System;
using System.Collections.Generic;

namespace BackendESP.Models;

public partial class Robot
{
    public int RobotId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Gamesession> GamesessionClientRobots { get; set; } = new List<Gamesession>();

    public virtual ICollection<Gamesession> GamesessionHostRobots { get; set; } = new List<Gamesession>();

    public virtual ICollection<PlayersRobot> PlayersRobots { get; set; } = new List<PlayersRobot>();
}
