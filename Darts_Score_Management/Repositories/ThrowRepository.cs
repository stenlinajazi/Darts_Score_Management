using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class ThrowRepository : BaseRepository<Throw>, IThrowRepository
    {
        public ThrowRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Throw>> GetThrowsForTurnAsync(int turnId)
        {
            return await _context.Throws
                .Where(t => t.TurnId == turnId)
                .OrderBy(t => t.ThrowNumber)
                .ToListAsync();
        }
    }
}
