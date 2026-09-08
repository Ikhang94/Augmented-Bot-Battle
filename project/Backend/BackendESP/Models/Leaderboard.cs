using System;
using System.Collections.Generic;

namespace BackendESP.Models;

public partial class Leaderboard
{
    public int LeaderboardId { get; set; }

    public int? PlayerId { get; set; }

    public int? Rank { get; set; }

    public virtual Player? Player { get; set; }
}
