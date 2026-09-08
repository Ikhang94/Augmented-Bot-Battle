using BackendESP.Data;
using BackendESP.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BackendESP.Controllers
{
    [Route("api/Leaderboard")]
    [ApiController]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardData _leaderboardData;

        public LeaderboardController(ILeaderboardData leaderboardData)
        {
            _leaderboardData = leaderboardData;
        }

        [HttpGet]
        [Route("GetLeadeboard")]
        public async Task<IResult> GetLeadeoard()
        {
            return await _leaderboardData.GetLeaderboard();
        }

        [HttpPost]
        [Route("CreateLeaderboard")]
        public async Task<IResult> CreateLeaderboard(int playerId)
        {
            return await _leaderboardData.CreateLeaderboard(playerId);
        }

        [HttpPut]
        [Route("SwapPlayers")]
        public async Task<IResult> SwapLeaderboard(List<LeaderboardDTO> players)
        {
            if (players.Count != 2)
                return TypedResults.BadRequest("You need 2 players to swap their ranks");
            return await _leaderboardData.SwapLeaderboard(players[0], players[1]);
        }
    }
}
