using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IStatisticRepository : IBaseRepository<Statistic>
    {
        Task<IEnumerable<Statistic>> GetPlayerGameStatisticsAsync(int gamePlayerId);
        Task UpdateStatisticsAsync(int gamePlayerId, List<Statistic> statistics);
    }
}
