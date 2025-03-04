using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.Enums;

namespace Darts_Score_Management.DTOs.Player
{
    public class PlayerStatsDTO
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int TotalLegsPlayed { get; set; }
        public string LegsWon { get; set; } // e.g., "6/11"
        public Dictionary<StatisticType, StatSummary> Last10LegsStats; // Average, Best for PPD, First9PPD, etc.
        public Dictionary<StatisticType, StatTotals> AllStats; // Totals and per-leg for 60+, 100+, 140+, 180+
    }
}
