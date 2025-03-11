using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface ILegStatsRepository : IBaseRepository<LegStats>
    {
        Task<LegStats> GetByLegAndPlayerAsync(int legId, int gamePlayerId);
        Task<IEnumerable<LegStats>> GetPlayerLegStatsAsync(int gamePlayerId);
        Task<IEnumerable<LegStats>> GetLast10LegsForPlayerAsync(int playerId);
        Task<IEnumerable<LegStats>> GetLegsForGameAsync(int gameId);
        //Task<IEnumerable<LegStats>> GetLegsForSetAsync(int setId);
        Task<IEnumerable<LegStats>> GetLegStatsForPlayerInSetAsync(int setId, int gamePlayerId, int playerId);
    }
}
