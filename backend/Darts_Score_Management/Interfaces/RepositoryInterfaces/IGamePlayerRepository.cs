using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IGamePlayerRepository : IBaseRepository<GamePlayer>
    {
        Task<IEnumerable<GamePlayer>> GetGamePlayersForGameAsync(int gameId);
        Task<GamePlayer> GetGamePlayerWithStatsAsync(int gamePlayerId);
    }
}
