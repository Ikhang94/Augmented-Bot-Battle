using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Services;
using Microsoft.AspNetCore.Mvc;
namespace BackendESP.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class PlayerController : Controller
    {
        private readonly IPlayerData _playerData;
        private readonly ITokenService _tokenService;
        public PlayerController(IPlayerData playerData, ITokenService tokenService)
        {
            _playerData = playerData;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IResult> CreatePlayer(PlayerDTO player)
        {
            //ヾ(＠⌒ー⌒＠)ノ \(@^0^@)/ ━━(￣ー￣*
            return await _playerData.CreatePlayerAsync(player);
        }//(┬┬﹏┬┬)

        [HttpGet]
        [Route("getall")]
        public async Task<IResult> GetAllPlayers()
        {
            return await _playerData.GetAllPlayersAsync();
        }

        [HttpGet("{playerID}")]
        public async Task<IResult> GetPlayerAsync(int playerID)
        {
            return await _playerData.GetPlayerAsync(playerID);
        }

        [HttpPut]
        public async Task<IResult> UpdatePlayerAsync(IEnumerable<PlayerDTO> playerDetails)
        {
            PlayerDTO oldPlayer = playerDetails.First();
            PlayerDTO newPlayer = playerDetails.Last();
            return await _playerData.UpdatePlayerAsync(oldPlayer, newPlayer);
        }

        [HttpDelete("{playerID}")]
        public async Task<IResult> DeletePlayerAsync(int playerID)
        {
            return await _playerData.DeletePlayerAsync(playerID);
        }

        [HttpPost("login")]
        public async Task<IResult> PlayerLogin(PlayerDTO player)
        {
            if (await _playerData.PlayerLogin(player) == TypedResults.Unauthorized())
                return TypedResults.Unauthorized();
            
            var token = _tokenService.GenerateToken(player);
            return TypedResults.Ok(new { token.Result });
        }

        [HttpPost]
        [Route("playerAcquireRobot")]
        public async Task<IResult> PlayerAcquireRobot(PlayersRobotDTO playersRobot)
        {
            return await _playerData.PlayerAcquireRobotAsync(playersRobot);
        }

        [HttpGet("GetPlayerRobots/{playerID}")]
        public async Task<IResult> GetPlayerRobotsAsync(int playerID)
        {
            return await _playerData.GetPlayerRobotsAsync(playerID);
        }

        [HttpGet("GetPlayerBy{email}")]
        public async Task<IResult> GetPlayerAsyncByEmail(string email)
        {
            return await _playerData.GetPlayerAsyncByEmail(email);
        }
    }
}
