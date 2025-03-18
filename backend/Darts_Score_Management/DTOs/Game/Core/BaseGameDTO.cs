using Darts_Score_Management.Enums;

namespace Darts_Score_Management.DTOs.Game.Core
{
    public class BaseGameDTO
    {
        public int Id { get; set; }
        public GameType Type { get; set; }
        public int StartingScore { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsComplete { get; set; }
    }
}
