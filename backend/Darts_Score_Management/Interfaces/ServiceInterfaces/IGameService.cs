using Darts_Score_Management.DTOs.Game.Core;
using Darts_Score_Management.DTOs.Game.Response;
using Darts_Score_Management.DTOs.Game.Statistics;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IGameService
    {
        Task<GameDTO> GetGameByIdAsync(int id);
        //Task<IEnumerable<GameDTO>> GetAllGamesAsync();
        Task<GameDTO> CreateGameAsync(CreateGameDTO createGameDto);
       // Task<GameDTO> UpdateGameAsync(int id, GameDTO gameDto);
        Task DeleteGameAsync(int id);
        Task<IEnumerable<PlayerGameSummaryDTO>> GetPlayerGamesAsync(int playerId);
        Task<GameDTO> EndGameAsync(int id, int winnerId);

        Task<IEnumerable<GameListResponseDTO>> GetAllSummariesAsync();
        Task<GameDetailsResponseDTO> GetGameWithDetailsAndHistoryAsync(int id);
        Task<int> GetActiveLegIdAsync();
    }
}
