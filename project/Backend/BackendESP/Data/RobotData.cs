using BackendESP.Data;
using BackendESP.DTO;
using BackendESP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BackendESP.Data
{
    public class RobotData : IRobotData 
    {
        private readonly PostgresDbContext _db;

        public RobotData(PostgresDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> GetAllRobotsAsync()
        {
            var robotsDTO = new List<RobotDTO>();
            try
            {
                var robotsModels = await _db.Robots.ToListAsync();
                foreach (var robot in robotsModels)
                {
                    robotsDTO.Add(new RobotDTO()
                    {
                        RobotID = robot.RobotId,
                        RobotName = robot.Name
                    });
                }

                return TypedResults.Ok(robotsDTO);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return TypedResults.InternalServerError(ex);
            }
        }

        public async Task<IResult> GetRobotAsync(int robotID)
        {
            try
            {
                var robot = await _db.Robots.FindAsync(robotID);
                if (robot == null)
                    return TypedResults.NotFound();

                var robotDTO = new RobotDTO()
                {
                    RobotID = robot.RobotId,
                    RobotName = robot.Name
                };
                return TypedResults.Ok(robotDTO);
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex);
            }
        }

        public async Task<IResult> CreateRobotAsync(RobotDTO robotDTO)
        {
            try
            {
                var robot = await _db.Robots.Where(r => r.Name == robotDTO.RobotName).FirstOrDefaultAsync();
                if (robot != null)
                    return TypedResults.BadRequest("robot already exists");
                var robotModel = new Robot()
                {
                    Name = robotDTO.RobotName
                };

                await _db.Robots.AddAsync(robotModel);
                await _db.SaveChangesAsync();

                return TypedResults.Ok("Robot created");

            }
            catch (Exception ex) 
            {
                return TypedResults.InternalServerError(ex);    
            }
        }

        public async Task<IResult> UpdateRobotAsync(RobotDTO oldRobotDTO, RobotDTO newRobotDTO)
        {
            try
            {
                var robot = await _db.Robots.Where(r => r.Name == oldRobotDTO.RobotName).FirstOrDefaultAsync();
                if (robot == null)
                    return TypedResults.NotFound("Old robot not found");

                robot.Name = newRobotDTO.RobotName;

                _db.Entry(robot).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return TypedResults.Ok("robot updated");
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex);
            }
        }

        public async Task<IResult> DeleteRobotAsync(int robotID)
        {
            try
            {
                var robot = await _db.Robots.FindAsync(robotID);
                if (robot == null)
                    return TypedResults.NotFound("robot not found");
                _db.Robots.Remove(robot);
                await _db.SaveChangesAsync();

                return TypedResults.Ok("robot deleted");
            }
            catch (Exception ex)
            {
                return TypedResults.InternalServerError(ex);
            }
        }
    }
}
