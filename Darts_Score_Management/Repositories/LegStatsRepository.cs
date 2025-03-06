using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class LegStatsRepository : BaseRepository<LegStats>, ILegStatsRepository
    {
        public LegStatsRepository(AppDbContext context) : base(context) { }

        public async Task<LegStats> GetByLegAndPlayerAsync(int legId, int gamePlayerId)
        {
            return await _context.LegStats
                .FirstOrDefaultAsync(ls => ls.LegId == legId && ls.GamePlayerId == gamePlayerId);
        }

        public async Task<IEnumerable<LegStats>> GetPlayerLegStatsAsync(int gamePlayerId)
        {
            return await _context.LegStats
                .Where(ls => ls.GamePlayerId == gamePlayerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LegStats>> GetLast10LegsForPlayerAsync(int gamePlayerId)
        {
            return await _context.LegStats
                .Include(ls => ls.Leg) // Eager load Leg for sorting by CreatedAt
                .Where(ls => ls.GamePlayerId == gamePlayerId)
                .OrderByDescending(ls => ls.Leg.Id)
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<LegStats>> GetLegsForGameAsync(int gameId)
        {
            return await _context.LegStats
                .Include(ls => ls.Leg).ThenInclude(l => l.Set)
                .Where(ls => ls.Leg.Set.GameId == gameId)
                .ToListAsync();
        }

        //public async Task<IEnumerable<LegStats>> GetLegsForSetAsync(int setId)
        //{
        //    return await _context.LegStats
        //        .Include(ls => ls.Leg)
        //        .Where(ls => ls.Leg.SetId == setId)
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<LegStats>> GetLegStatsForPlayerInSetAsync(int setId, int gamePlayerId, int playerId)
        {
            return await _context.LegStats
                .Include(ls => ls.Leg)
                .Where(ls => ls.Leg.SetId == setId && ls.GamePlayerId == gamePlayerId)
                .ToListAsync();
        }
    }
}
