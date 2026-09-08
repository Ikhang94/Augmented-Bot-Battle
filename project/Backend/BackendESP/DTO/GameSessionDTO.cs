namespace BackendESP.DTO
{
    public class GameSessionDTO
    {
        public int Id { get; set; }
        public int? HostPlayerId { get; set; }
        public int? ClientPlayerId { get; set; }
        public int? HostRobotId { get; set; }
        public int? ClientRobotId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
