using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Statistic;

namespace Darts_Score_Management.DTOs.GamePlayer
{
    public class GamePlayerDTO
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public PlayerDTO Player { get; set; }
        public int TurnOrder { get; set; }
        public bool IsWinner { get; set; }
        public int FinalRanking { get; set; }
        public List<StatisticDTO> Statistics { get; set; }
    }
}
