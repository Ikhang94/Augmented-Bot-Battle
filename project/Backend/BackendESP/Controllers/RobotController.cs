using BackendESP.Data;
using BackendESP.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BackendESP.Controllers
{
    [Route("api/robots")]
    [ApiController]
    public class RobotController : Controller
    {
        private readonly IRobotData _robotData;

        public RobotController(IRobotData robotData)
        {
            _robotData = robotData;
        }

        [HttpGet]
        public async Task<IResult> GetAllRobots()
        {
            return await _robotData.GetAllRobotsAsync();
        }

        [HttpPost]
        public async Task<IResult> CreateRobot(RobotDTO robot)
        {
            return await _robotData.CreateRobotAsync(robot);
        }

        [HttpGet("{robotID}")]
        public async Task<IResult> GetRobotAsync(int robotID)
        {
            return await _robotData.GetRobotAsync(robotID);
        }

        [HttpPut]
        public async Task<IResult> UpdateRobotAsync(IEnumerable<RobotDTO> robots)
        {
            RobotDTO oldRobotDTO = robots.First();
            RobotDTO newRobotDTO = robots.Last();
            return await _robotData.UpdateRobotAsync(oldRobotDTO, newRobotDTO);
        }

        [HttpDelete("{robotID}")]
        public async Task<IResult> DeleteRobotAsync(int robotID)
        {
            return await _robotData.DeleteRobotAsync(robotID);
        }

    }
}
