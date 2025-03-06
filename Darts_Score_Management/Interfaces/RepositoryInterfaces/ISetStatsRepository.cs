using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface ISetStatsRepository : IBaseRepository<SetStats>
    {
        Task<SetStats> GetBySetAndPlayerAsync(int setId, int gamePlayerId);
        Task<IEnumerable<SetStats>> GetPlayerSetStatsAsync(int gamePlayerId);
        Task<IEnumerable<SetStats>> GetSetsForGameAsync(int gameId);
    }
}
