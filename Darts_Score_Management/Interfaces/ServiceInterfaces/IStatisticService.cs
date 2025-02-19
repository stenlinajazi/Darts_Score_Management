using Darts_Score_Management.DTOs.Statistic;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IStatisticService
    {
        Task<StatisticDTO> GetStatisticByIdAsync(int id);
        Task<IEnumerable<StatisticDTO>> GetPlayerGameStatisticsAsync(int gamePlayerId);
        Task<IEnumerable<StatisticDTO>> UpdateStatisticsAsync(int gamePlayerId, List<StatisticDTO> stats);
    }
}
