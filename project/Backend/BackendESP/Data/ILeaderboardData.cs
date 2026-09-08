using BackendESP.DTO;

namespace BackendESP.Data
{
    public interface ILeaderboardData
    {
        Task<IResult> GetLeaderboard();
        Task<IResult> CreateLeaderboard(int playerId);
        Task<IResult> SwapLeaderboard(LeaderboardDTO player1, LeaderboardDTO player2);
    }
}
