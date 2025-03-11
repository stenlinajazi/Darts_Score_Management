using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface ITurnRepository : IBaseRepository<Turn>
    {
        Task<Turn> GetTurnWithThrowsAsync(int id);
        Task<Turn> AddTurnWithThrowsAsync(Turn turn);
        Task<Turn> GetLastTurnByLegAsync(int legId);
        Task<Turn> GetLastTurnByPlayerAndLegAsync(int playerId, int legId);
        Task<IEnumerable<Turn>> GetTurnsByPlayerAndLegAsync(int playerId, int legId);
    }
}
