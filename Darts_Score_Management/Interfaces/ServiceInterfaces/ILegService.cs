using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Leg;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface ILegService
    {
        Task<Leg> GetLegByIdAsync(int id);
        Task<LegDTO> CreateLegAsync(CreateLegDTO createLegDto);
        Task<LegDTO> EndLegAsync(int id, int winnerId);
        Task<IEnumerable<LegDTO>> GetLegsBySetIdAsync(int setId);
    }
}
