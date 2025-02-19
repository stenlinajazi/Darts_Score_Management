using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IPlayerRepository : IBaseRepository<Player>
    {
        Task<Player> GetPlayerWithStatsAsync(int id);
        Task<IEnumerable<Player>> GetActivePlayersAsync();
    }
}
