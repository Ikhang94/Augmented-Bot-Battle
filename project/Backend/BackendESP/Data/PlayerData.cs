using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendESP.Data
{
    public class PlayerData : IPlayerData
    {
        private readonly PostgresDbContext _db;
        public PlayerData(PostgresDbContext db)
        {
            _db = db;
        }
        public async Task<IResult> GetAllPlayersAsync()
        {
            var players_dto = new List<PlayerDTO>();
            try
            {
                var players_models = await _db.Players.ToListAsync();
                foreach (var player in players_models)
                {
                    players_dto.Add(new PlayerDTO()
                    {
                        PlayerID = player.PlayerId,
                        UserName = player.Username,
                        Email = player.Email,
                        Password = player.Password
                    });
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return TypedResults.InternalServerError(ex);
            }

            return TypedResults.Ok(players_dto);
        }

        public async Task<IResult> GetPlayerAsync(int playerID)
        {
            try
            {
                var player = await _db.Players.FindAsync(playerID);

                if (player == null)
                    return TypedResults.NotFound("pas trouvé connard");

                var playerDTO = new PlayerDTO()
                {
                    PlayerID = playerID,
                    UserName = player.Username,
                    Email = player.Email,
                    Password = player.Password

                };
                return TypedResults.Ok(playerDTO);

            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex);
            }
        }

        public async Task<IResult> GetPlayerAsyncByEmail(string email)
        {
            try
            {
                var player = await _db.Players.FirstOrDefaultAsync(x => x.Email == email);

                if (player == null)
                    return TypedResults.NotFound("pas trouvé connard");

                var playerDTO = new PlayerDTO()
                {
                    PlayerID = player.PlayerId,
                    UserName = player.Username,
                    Email = player.Email,
                    Password = player.Password

                };
                return TypedResults.Ok(playerDTO);

            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex);
            }

        }

        public async Task<PlayerDTO> GetPlayerAsyncByEmailToken(string email)
        {
            try
            {
                var player = await _db.Players.FirstOrDefaultAsync(x => x.Email == email);

                if (player == null)
                    return null;

                var playerDTO = new PlayerDTO()
                {
                    PlayerID = player.PlayerId,
                    UserName = player.Username,
                    Email = player.Email,
                    Password = player.Password

                };
                return playerDTO;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<IResult> CreatePlayerAsync(PlayerDTO player)
        {
            if (player.UserName == null || player.Email == null && player.Password == null)
            {
                return TypedResults.NotFound("ça ne marche pas connard");
            }

            var playerModel = new Player()
            {
                Username = player.UserName,
                Email = player.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(player.Password)
            };
            try
            {
                await _db.Players.AddAsync(playerModel);
                await _db.SaveChangesAsync();
                return TypedResults.Ok("ça marche connard");
            } catch (Exception ex) {
                return TypedResults.InternalServerError(ex);
            }
        }

        public async Task<IResult> UpdatePlayerAsync(PlayerDTO oldPlayer, PlayerDTO newPlayer)
        {
            try
            {
                var player = await _db.Players.Where(p => p.Email == oldPlayer.Email).FirstOrDefaultAsync();
                if (player == null)
                    return TypedResults.BadRequest("pas toruvé connard");

                player.Email = newPlayer.Email;
                player.Password = newPlayer.Password;
                player.Username = newPlayer.UserName;

                _db.Players.Entry(player).State = EntityState.Modified;

                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.Message);
            }

            return TypedResults.Ok("Player updated");
        }

        public async Task<IResult> DeletePlayerAsync(int playerID)
        {
            try
            {
                var player = await _db.Players.FindAsync(playerID);
                if (player == null)
                    return TypedResults.NotFound("pas toruvé connard");
                _db.Players.Remove(player);
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.Message);
            }

            return TypedResults.Ok();
        }

        public async Task<IResult> PlayerLogin(PlayerDTO playerDTO)
        {
            var user = await _db.Players.SingleOrDefaultAsync(u =>
                u.Email == playerDTO.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(playerDTO.Password, user.Password))
                return TypedResults.Unauthorized();

            return TypedResults.Ok();
        }

        public async Task<IResult> PlayerAcquireRobotAsync(PlayersRobotDTO playerRobot)
        {
            try
            {
                var p = await _db.Players.FindAsync(playerRobot.PlayerId);
                var r = await _db.Robots.FindAsync(playerRobot.RobotId);

                if (p == null || r == null)
                    return TypedResults.NotFound("player or robot doesn't exists");

                var newPlayerRobot = new PlayersRobot()
                {
                    PlayerId = playerRobot.PlayerId,
                    RobotId = playerRobot.RobotId,
                };

                await _db.PlayersRobots.AddAsync(newPlayerRobot);
                await _db.SaveChangesAsync();
                return TypedResults.Ok(playerRobot);
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.Message);
            }
        }

        public async Task<IResult> GetPlayerRobotsAsync(int playerID)
        {
            try
            {
                if (await _db.Players.FindAsync(playerID) == null)
                    return TypedResults.NotFound("Player doesn't exists");
                var playerRobots = await _db.PlayersRobots.Where(r => r.PlayerId == playerID).ToListAsync();
                List<RobotDTO> robots = new List<RobotDTO>();
                foreach (var playerRobot in playerRobots)
                {
                    var robot = await _db.Robots.FindAsync(playerRobot.RobotId);
                    robots.Add(new RobotDTO()
                    {
                        RobotID = robot.RobotId,
                        RobotName = robot.Name
                    });
                }

                return TypedResults.Ok(robots);
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex.Message);
            }
        }
    }
}
