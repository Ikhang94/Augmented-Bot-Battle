using System;
using System.Collections.Generic;

namespace BackendESP.Models;

public partial class PlayersRobot
{
    public int Id { get; set; }

    public int? PlayerId { get; set; }

    public int? RobotId { get; set; }

    public virtual Player? Player { get; set; }

    public virtual Robot? Robot { get; set; }
}
