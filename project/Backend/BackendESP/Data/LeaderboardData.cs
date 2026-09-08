using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendESP.Data
{
    public class LeaderboardData : ILeaderboardData
    {
        private readonly PostgresDbContext _db;

        public LeaderboardData(PostgresDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> GetLeaderboard()
        {
            var leaderboard = new List<LeaderboardDTO>();
            try
            {
                var board = await _db.Leaderboards.OrderBy(r => r.Rank).ToListAsync();
                foreach (var item in board)
                {
                    leaderboard.Add(new LeaderboardDTO
                    {
                        LeaderboardId = item.LeaderboardId,
                        Rank = item.Rank,
                        PlayerId = item.PlayerId
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return TypedResults.InternalServerError(ex);
            }

            return TypedResults.Ok(leaderboard);
        }

        public async Task<IResult> CreateLeaderboard(int playerId)
        {
            try
            {
                var entry = await _db.Leaderboards.FindAsync(playerId);
                if (entry != null)
                    return TypedResults.BadRequest("player already exists");
                int count = await _db.Leaderboards.CountAsync();
                var newLeaderboard = new Leaderboard()
                {
                    PlayerId = playerId,
                    Rank = count + 1
                };

                await _db.Leaderboards.AddAsync(newLeaderboard);
                await _db.SaveChangesAsync();
                return TypedResults.Ok(newLeaderboard);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return TypedResults.InternalServerError(ex.ToString());
            }
        }

        public async Task<IResult> SwapLeaderboard(LeaderboardDTO player1, LeaderboardDTO player2)
        {
            try
            {
                var p1 = await _db.Leaderboards.FindAsync(player1.PlayerId);
                if (p1 == null)
                    return TypedResults.BadRequest("Player 1 isn't on the leaderboard");
                var p2 = await _db.Leaderboards.FindAsync(player2.PlayerId);
                if (p2 == null)
                    return TypedResults.BadRequest("Player 2 isn't on the leaderboard");

                p1.Rank = player2.Rank;
                p2.Rank = player1.Rank;

                _db.Leaderboards.Entry(p1).State = EntityState.Modified;
                _db.Leaderboards.Entry(p2).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return TypedResults.Ok("players rank swapped");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return TypedResults.InternalServerError(ex.Message);
            }
        }
    }
}
