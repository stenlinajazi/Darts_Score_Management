using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface ITurnRepository : IBaseRepository<Turn>
    {
        Task<Turn> GetTurnWithThrowsAsync(int id);
        Task<Turn> AddTurnWithThrowsAsync(Turn turn);
    }
}
