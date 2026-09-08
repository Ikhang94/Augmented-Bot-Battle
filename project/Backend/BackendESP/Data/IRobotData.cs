using BackendESP.Data;
using BackendESP.DTO;
using Microsoft.AspNetCore.Http;


namespace BackendESP.Data
{
    public interface IRobotData
    {
        Task<IResult> GetAllRobotsAsync();
        Task<IResult> GetRobotAsync(int robotId);
        Task<IResult> CreateRobotAsync(RobotDTO robotDTO);
        Task<IResult> UpdateRobotAsync(RobotDTO oldRobotDTO, RobotDTO newRobotDTO);
        Task<IResult> DeleteRobotAsync(int robotId);
    }
}
