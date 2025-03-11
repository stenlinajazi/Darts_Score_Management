using Darts_Score_Management.Data.Models.Bases;

namespace Darts_Score_Management.Data.Models
{
    public class GamePlayer : BaseModel
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int TurnOrder { get; set; }
        public bool IsWinner { get; set; }
        public int FinalRanking { get; set; }

        // Navigation properties
        public Game Game { get; set; }
        public Player Player { get; set; }
        public List<Statistic> Statistics { get; set; }
        public List<LegStats> LegStats { get; set; }
        public List<SetStats> SetStats { get; set; }
        public List<GameStats> GameStats { get; set; }
    }
}
