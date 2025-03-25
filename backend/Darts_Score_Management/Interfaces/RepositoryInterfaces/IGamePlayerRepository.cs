using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.GamePlayer;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IGamePlayerRepository : IBaseRepository<GamePlayer>
    {
        Task<IEnumerable<GamePlayerDTO>> GetGamePlayersForGameAsync(int gameId);
        Task<GamePlayerDTO> GetGamePlayerAsync(int gamePlayerId);
    }
}
