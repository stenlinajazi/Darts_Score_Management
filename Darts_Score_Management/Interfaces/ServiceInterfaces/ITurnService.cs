using Darts_Score_Management.DTOs.Turn;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface ITurnService
    {
        Task<TurnDTO> GetTurnByIdAsync(int id);
        Task<TurnDTO> CreateTurnAsync(CreateTurnDTO createTurnDto);
        Task<IEnumerable<TurnDTO>> GetTurnsByLegIdAsync(int legId);
    }
}
