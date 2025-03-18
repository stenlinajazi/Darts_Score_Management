using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IGameStatsRepository:IBaseRepository<GameStats>
    {
        Task<GameStats> GetByGameAndPlayerAsync(int gameId, int gamePlayerId);
        Task<IEnumerable<GameStats>> GetPlayerGameStatsAsync(int gamePlayerId);
        Task<IEnumerable<GameStats>> GetGameStatsForPlayerAsync(int playerId);
    }
}
