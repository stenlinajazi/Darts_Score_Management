using Darts_Score_Management.DTOs.Set;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface ISetService
    {
        Task<SetDTO> GetSetByIdAsync(int id);
        Task<SetDTO> CreateSetAsync(CreateSetDTO createSetDto);
        Task<SetDTO> EndSetAsync(int id, int winnerId);
        Task<IEnumerable<SetDTO>> GetSetsByGameIdAsync(int gameId);
    }
}
