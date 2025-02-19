using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(AppDbContext context) : base(context) { }

        public async Task<Player> GetPlayerWithStatsAsync(int id)
        {
            return await _context.Players
                .Include(p => p.GamePlayers)
                    .ThenInclude(gp => gp.Statistics)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Player>> GetActivePlayersAsync()
        {
            return await _context.Players
                .Where(p => p.IsActive)
                .ToListAsync();
        }
    }
}
