using Darts_Score_Management.Enums;

namespace Darts_Score_Management.DTOs.Game.Statistics
{
    public class PlayerGameSummaryDTO
    {
        public int GameId { get; set; }
        public GameType Type { get; set; }
        public int StartingScore { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsComplete { get; set; }
        public int PlayerCount { get; set; }
        public int SetsCount { get; set; }
        public bool WasWinner { get; set; }
        public int FinalRanking { get; set; }
        public int SetsWon { get; set; }
        public int LegsWon { get; set; }
    }
}
