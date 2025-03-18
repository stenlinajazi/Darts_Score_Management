using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class SetRepository : BaseRepository<Set>, ISetRepository
    {
        public SetRepository(AppDbContext context) : base(context) { }

        public async Task<Set> GetSetWithLegsAsync(int id)
        {
            return await _context.Sets
                .Include(s => s.Legs)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Set>> GetSetsForGameAsync(int gameId)
        {
            return await _context.Sets
                .Include(s => s.Legs)
                .Where(s => s.GameId == gameId)
                .OrderBy(s => s.SetNumber)
                .ToListAsync();
        }
    }
}
