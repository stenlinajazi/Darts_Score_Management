using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface ILegRepository : IBaseRepository<Leg>
    {
        Task<Leg> GetLegWithDetailsAsync(int id);
        Task<List<Turn>> GetLegTurnsAsync(int legId);
        Task<List<GamePlayer>> GetGamePlayersForLegAsync(int legId);
    }
}
