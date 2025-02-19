using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class StatisticRepository : BaseRepository<Statistic>, IStatisticRepository
    {
        public StatisticRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Statistic>> GetPlayerGameStatisticsAsync(int gamePlayerId)
        {
            return await _context.Statistics
                .Where(s => s.GamePlayerId == gamePlayerId)
                .ToListAsync();
        }

        public async Task UpdateStatisticsAsync(int gamePlayerId, List<Statistic> statistics)
        {
            var existingStats = await _context.Statistics
                .Where(s => s.GamePlayerId == gamePlayerId)
                .ToListAsync();

            _context.Statistics.RemoveRange(existingStats);
            await _context.Statistics.AddRangeAsync(statistics);
            await _context.SaveChangesAsync();
        }
    }
}
