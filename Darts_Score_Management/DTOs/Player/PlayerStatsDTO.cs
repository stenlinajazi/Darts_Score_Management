using Darts_Score_Management.DTOs.Game;

namespace Darts_Score_Management.DTOs.Player
{
    public class PlayerStatsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }

        // Game statistics
        public int TotalGamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int SetsWon { get; set; }
        public int LegsWon { get; set; }

        // Performance statistics per game
        public ICollection<GameStatisticsDTO> GameStatistics { get; set; }

        // Career averages
        public CareerAveragesDTO CareerAverages { get; set; }
    }
}
