using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class LegRepository : BaseRepository<Leg>, ILegRepository
    {
        public LegRepository(AppDbContext context) : base(context) { }

        public async Task<Leg> GetLegWithDetailsAsync(int id)
        {
            return await _context.Legs
                .Include(l => l.Turns)
                    .ThenInclude(t => t.Throws)
                .Include(l => l.Winner)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Turn>> GetLegTurnsAsync(int legId)
        {
            return await _context.Turns
                .Include(t => t.Throws)
                .Where(t => t.LegId == legId)
                .OrderBy(t => t.TurnNumber)
                .ToListAsync();
        }
    }
}
