using Darts_Score_Management.Enums;

namespace Darts_Score_Management.DTOs.Game
{
    public class GameSummaryDTO
    {
        public int Id { get; set; }
        public GameType Type { get; set; }
        public int StartingScore { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsComplete { get; set; }
        public int PlayerCount { get; set; }
        public int SetsCount { get; set; }
        public int? WinnerId { get; set; }
    }
}
