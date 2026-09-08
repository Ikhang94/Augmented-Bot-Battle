using BackendESP.Data; 
using BackendESP.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace BackendESP.Data
{
    public interface IPlayerData
    {
        Task<IResult> GetAllPlayersAsync();
        Task<IResult> CreatePlayerAsync(PlayerDTO player);
        Task<IResult> GetPlayerAsync(int playerID);
        Task<IResult> UpdatePlayerAsync(PlayerDTO oldPlayer, PlayerDTO newPlayer);
        Task<IResult> DeletePlayerAsync(int playerID);
        Task<IResult> PlayerLogin(PlayerDTO player);
        Task<IResult> PlayerAcquireRobotAsync(PlayersRobotDTO playerRobot);
        Task<IResult> GetPlayerRobotsAsync(int playerID);
        Task<IResult> GetPlayerAsyncByEmail(string email);
        Task<PlayerDTO> GetPlayerAsyncByEmailToken(string email);
    }
}