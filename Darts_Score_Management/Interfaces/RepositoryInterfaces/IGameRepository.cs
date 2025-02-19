using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IGameRepository : IBaseRepository<Game>
    {
        Task<Game> GetGameWithDetailsAsync(int id);
        Task<IEnumerable<Game>> GetPlayerGamesAsync(int playerId);
        // Task<Game> CreateGameAsync(Game game, List<int> playerIds);
        Task<Game> CreateGameWithPlayersAsync(Game game, IEnumerable<GamePlayer> gamePlayers);
    }
}
