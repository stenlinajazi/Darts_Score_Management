using Darts_Score_Management.DTOs.Player;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IPlayerService
    {
        Task<PlayerDTO> GetPlayerByIdAsync(int id);
        Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync();
        Task<PlayerDTO> CreatePlayerAsync(UpsertPlayerDTO createPlayerDto);
        Task<PlayerDTO> UpdatePlayerAsync(int id, UpsertPlayerDTO playerDto);
        Task DeletePlayerAsync(int id);
        Task<IEnumerable<PlayerDTO>> GetActivePlayersAsync();
        //Task<PlayerStatsDTO> GetPlayerWithStatsAsync(int id);
    }
}
