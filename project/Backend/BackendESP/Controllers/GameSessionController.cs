using BackendESP.Data;
using BackendESP.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BackendESP.Controllers
{
    [Route("api/GameSessions")]
    [ApiController]
    public class GameSessionController : Controller
    {
        private readonly IGameSessionData _gameSessionData;

        public GameSessionController(IGameSessionData gameSessionData)
        {
            _gameSessionData = gameSessionData;
        }

        [HttpGet]
        [Route("getAllGameSessions")]
        public async Task<IResult> GetAllGameSessionsAsync()
        {
            return await _gameSessionData.GetAllGameSessionsAsync();
        }

        [HttpGet]
        [Route("GetGameSession")]
        public async Task<IResult> GetGameSessionAsync(int id)
        {
            return await _gameSessionData.GetGameSessionAsync(id);
        }

        [HttpPost]
        [Route("CreateGameSession")]
        public async Task<IResult> CreateGameSessionAsync(GameSessionDTO gameSessionDTO)
        {
            return await _gameSessionData.CreateGameSessionAsync(gameSessionDTO);
        }

        [HttpPut]
        [Route("UpdateGameSession")]
        public async Task<IResult> UpdateGameSessionAsync(GameSessionDTO gameSessionDTO)
        {
            return await _gameSessionData.UpdateGameSessionAsync(gameSessionDTO);
        }

        [HttpDelete]
        [Route("DeleteGameSession")]
        public async Task<IResult> DeleteGameSessionAsync(int id)
        {
            return await _gameSessionData.DeleteGameSessionAsync(id);
        }

        [HttpGet]
        [Route("GetPlayerGameSessions")]
        public async Task<IResult> GetGameSessionsOfPlayerAsync(int playerId)
        {
            return await _gameSessionData.GetGameSessionsOfPlayer(playerId);
        }
    }
}
