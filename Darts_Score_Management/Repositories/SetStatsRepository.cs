using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class SetStatsRepository : BaseRepository<SetStats>, ISetStatsRepository
    {
        public SetStatsRepository(AppDbContext context) : base(context) { }

        public async Task<SetStats> GetBySetAndPlayerAsync(int setId, int gamePlayerId)
        {
            return await _context.SetStats
                .FirstOrDefaultAsync(ss => ss.SetId == setId && ss.GamePlayerId == gamePlayerId);
        }

        public async Task<IEnumerable<SetStats>> GetPlayerSetStatsAsync(int gamePlayerId)
        {
            return await _context.SetStats
                .Where(ss => ss.GamePlayerId == gamePlayerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SetStats>> GetSetsForGameAsync(int gameId)
        {
            return await _context.SetStats
                .Include(ss => ss.Set)
                .Where(ss => ss.Set.GameId == gameId)
                .ToListAsync();
        }
    }
}
