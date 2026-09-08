using BackendESP.DTO;

namespace BackendESP.Data
{
    public interface IGameSessionData
    {
        Task<IResult> GetAllGameSessionsAsync();
        Task<IResult> GetGameSessionAsync(int GameSessionID);
        Task<IResult> CreateGameSessionAsync(GameSessionDTO gameSessionDTO);
        Task<IResult> UpdateGameSessionAsync(GameSessionDTO gameSessionDTO);
        Task<IResult> DeleteGameSessionAsync(int id);
        Task<IResult> GetGameSessionsOfPlayer(int playerId);
    }
}
