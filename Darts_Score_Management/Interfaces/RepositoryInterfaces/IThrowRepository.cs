using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IThrowRepository : IBaseRepository<Throw>
    {
        Task<IEnumerable<Throw>> GetThrowsForTurnAsync(int turnId);
    }
}
