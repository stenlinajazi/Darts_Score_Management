using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IGameRepository : IBaseRepository<Game>
    {
        Task<Game> GetGameWithDetailsAsync(int id);
        Task<IEnumerable<PlayerGameSummaryDTO>> GetPlayerGamesAsync(int playerId);
        // Task<Game> CreateGameAsync(Game game, List<int> playerIds);
        Task<Game> CreateGameWithPlayersAsync(Game game, IEnumerable<GamePlayer> gamePlayers);
        Task<IEnumerable<Game>> GetAllSummariesAsync();
    }
}
