using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Player;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IPlayerRepository : IBaseRepository<Player>
    {
        //Task<Player> GetPlayerWithStatsAsync(int id);
        Task<IEnumerable<Player>> GetActivePlayersAsync();
        Task<IEnumerable<PlayerDTO>> GetAllPlayerDTOsAsync();
    }
}
