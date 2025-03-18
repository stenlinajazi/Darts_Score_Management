using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game.Statistics;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Statistic;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IStatisticService
    {
        Task UpdateLegStatsAsync(int legId, List<GamePlayer> gamePlayers);
        Task UpdateSetStatsAsync(int setId, Dictionary<int, int> legsWonPerPlayer);
        Task UpdateGameStatsAsync(int gameId, Dictionary<int, int> setsWonPerPlayer);
        Task<LegStatsDTO> GetLegStatsAsync(int legId, int gamePlayerId);
        Task<SetStatsDTO> GetSetStatsAsync(int setId, int gamePlayerId);
        Task<GameStatsDTO> GetGameStatsAsync(int gameId, int gamePlayerId);
        Task<PlayerStatsDTO> GetPlayerStatsAsync(int playerId);
    }
}
