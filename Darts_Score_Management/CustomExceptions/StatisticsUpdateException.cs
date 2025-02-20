using Darts_Score_Management.Enums;

namespace Darts_Score_Management.CustomExceptions
{
    public class StatisticsUpdateException : Exception
    {
        public int GamePlayerId { get; }
        public StatisticType StatisticType { get; }

        public StatisticsUpdateException(string message, int gamePlayerId, StatisticType statisticType)
            : base(message)
        {
            GamePlayerId = gamePlayerId;
            StatisticType = statisticType;
        }
    }
}
