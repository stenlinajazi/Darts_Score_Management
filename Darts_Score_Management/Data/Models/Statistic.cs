using Darts_Score_Management.Data.Models.Bases;
using Darts_Score_Management.Enums;

namespace Darts_Score_Management.Data.Models
{
    public class Statistic : BaseModel
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; }
        public StatisticType Type { get; set; } // Enum: AveragePerTurn, CheckoutPercentage, etc.
        public string Value { get; set; } // Could be numeric or JSON for complex stats

        // Navigation properties
        public GamePlayer GamePlayer { get; set; }
    }
}
