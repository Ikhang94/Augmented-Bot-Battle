using System;
using System.Collections.Generic;

namespace BackendESP.Models;

public partial class Gamesession
{
    public int Id { get; set; }

    public int? HostPlayerId { get; set; }

    public int? ClientPlayerId { get; set; }

    public int? HostRobotId { get; set; }

    public int? ClientRobotId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual Player? ClientPlayer { get; set; }

    public virtual Robot? ClientRobot { get; set; }

    public virtual Player? HostPlayer { get; set; }

    public virtual Robot? HostRobot { get; set; }
}
