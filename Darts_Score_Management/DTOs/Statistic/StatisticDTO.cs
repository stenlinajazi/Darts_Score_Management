using Darts_Score_Management.Enums;

namespace Darts_Score_Management.DTOs.Statistic
{
    public class StatisticDTO
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; }
        public StatisticType Type { get; set; }
        public string Value { get; set; }
    }
}
