using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Statistic;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IGamePlayerService
    {
        Task<GamePlayerDTO> GetGamePlayerByIdAsync(int id);
        Task<IEnumerable<GamePlayerDTO>> GetGamePlayersByGameIdAsync(int gameId);
        //Task<GamePlayerDTO> UpdateGamePlayerStatsAsync(int id, List<StatisticDTO> stats);
        //Task<GamePlayerDTO> SetGamePlayerWinnerAsync(int gameId, int playerId);
    }
}
