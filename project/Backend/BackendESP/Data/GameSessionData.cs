using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendESP.Data
{
    public class GameSessionData : IGameSessionData
    {
        private readonly PostgresDbContext _db;
        public GameSessionData(PostgresDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> GetAllGameSessionsAsync()
        {
            var gameSessions_dto = new List<GameSessionDTO>();
            try
            {
                var gameSessions_models = await _db.Gamesessions.ToListAsync();
                foreach (var gameSession in gameSessions_models)
                {
                    gameSessions_dto.Add(new GameSessionDTO()
                    {
                        Id = gameSession.Id,
                        ClientPlayerId = gameSession.ClientPlayerId,
                        HostPlayerId = gameSession.HostPlayerId,
                        HostRobotId = gameSession.HostRobotId,
                        ClientRobotId = gameSession.ClientRobotId,
                        StartTime = gameSession.StartTime,
                        EndTime = gameSession.EndTime,
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return TypedResults.InternalServerError(ex);
            }

            return TypedResults.Ok(gameSessions_dto);
        }

        public async Task<IResult> GetGameSessionAsync(int GameSessionID)
        {
            try
            {
                var gameSession = await _db.Gamesessions.FirstOrDefaultAsync(x => x.Id == GameSessionID);

                var gameSessionDTO = new GameSessionDTO()
                {
                    Id = gameSession!.Id,
                    HostPlayerId = gameSession.HostPlayerId,
                    HostRobotId = gameSession.HostRobotId,
                    ClientPlayerId = gameSession.ClientPlayerId,
                    ClientRobotId = gameSession.ClientRobotId,
                    StartTime = gameSession.StartTime,
                    EndTime = gameSession.EndTime,
                };
                return TypedResults.Ok(gameSessionDTO);
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex);
            }
        }

        public async Task<IResult> CreateGameSessionAsync(GameSessionDTO gameSessionDTO)
        {
            var gameSessionModel = new Gamesession()
            {
                HostPlayerId = gameSessionDTO.HostPlayerId,
                HostRobotId = gameSessionDTO.HostRobotId,
                ClientPlayerId = gameSessionDTO.ClientPlayerId,
                ClientRobotId = gameSessionDTO.ClientRobotId,
                StartTime = gameSessionDTO.StartTime,
                EndTime = gameSessionDTO.EndTime,
            };
            try
            {
                await _db.Gamesessions.AddAsync(gameSessionModel);
                await _db.SaveChangesAsync();
                return TypedResults.Ok("GameSession Created");
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.ToString());
            }
        }

        public async Task<IResult> UpdateGameSessionAsync(GameSessionDTO gameSessionDTO)
        {
            try
            {
                var gameSession = await _db.Gamesessions.Where(g => g.Id == gameSessionDTO.Id).FirstOrDefaultAsync();
                if (gameSession == null)
                    return TypedResults.NotFound("not found");

                gameSession.HostPlayerId = gameSessionDTO.HostPlayerId;
                gameSession.HostRobotId = gameSessionDTO.HostRobotId;
                gameSession.ClientPlayerId = gameSessionDTO.ClientPlayerId;
                gameSession.ClientRobotId = gameSessionDTO.ClientRobotId;
                gameSession.StartTime = gameSessionDTO.StartTime;
                gameSession.EndTime = gameSessionDTO.EndTime;

                _db.Gamesessions.Entry(gameSession).State = EntityState.Modified;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.Message);
            }
            return TypedResults.Ok("Game session updated");
        }

        public async Task<IResult> DeleteGameSessionAsync(int id)
        {
            try
            {
                var gameSession = await _db.Gamesessions.FindAsync(id);
                if (gameSession == null)
                    return TypedResults.NotFound();
                _db.Gamesessions.Remove(gameSession);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.Message);
            }

            return TypedResults.Ok();
        }

        public async Task<IResult> GetGameSessionsOfPlayer(int playerId)
        {
            List<GameSessionDTO> gameSessionDTOs = new List<GameSessionDTO>();
            try
            {
                var gameSessions = await _db.Gamesessions.Where(g => g.HostPlayerId == playerId || g.ClientPlayerId == playerId).ToListAsync();
                if (gameSessions == null)
                    return TypedResults.NotFound("player not found");
                foreach (var gameSession in gameSessions)
                {
                    gameSessionDTOs.Add(new GameSessionDTO()
                    {
                        Id = gameSession.Id,
                        HostPlayerId = gameSession.HostPlayerId,
                        ClientPlayerId = gameSession.ClientPlayerId,
                        HostRobotId = gameSession.HostRobotId,
                        ClientRobotId = gameSession.ClientRobotId,
                        StartTime = gameSession.StartTime,
                        EndTime = gameSession.EndTime,
                    });
                }

                return TypedResults.Ok(gameSessionDTOs);
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.Message);
            }
        }
    }
}
