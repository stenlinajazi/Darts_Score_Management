using Darts_Score_Management.DTOs.Statistic;

namespace Darts_Score_Management.DTOs.Game
{
    public class GameStatisticsDTO
    {
        public int GameId { get; set; }
        public DateTime GameDate { get; set; }
        public bool IsWinner { get; set; }
        public List<StatisticDTO> Statistics { get; set; }
    }
}
