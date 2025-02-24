using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface ITurnService
    {
        //Task<TurnDTO> GetTurnByIdAsync(int id);
        Task<TurnDTO> CreateTurnAsync(CreateTurnDTO createTurnDto);
        //Task<IEnumerable<TurnDTO>> GetTurnsByLegIdAsync(int legId);
        Task<TurnDTO> AddThrowToTurnAsync(int turnId, CreateThrowDTO throwDto);
        //Task GetTurnByIdAsync(int turnId);
        Task<TurnDTO> GetTurnByIdAsync(int turnId);
        Task<TurnDTO> GetLastTurnByLegAsync(int legId);
    }
}
