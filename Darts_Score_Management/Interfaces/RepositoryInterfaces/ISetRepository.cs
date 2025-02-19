using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface ISetRepository : IBaseRepository<Set>
    {
        Task<Set> GetSetWithLegsAsync(int id);
        Task<IEnumerable<Set>> GetSetsForGameAsync(int gameId);
    }
}
