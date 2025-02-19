using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class GamePlayerRepository : BaseRepository<GamePlayer>, IGamePlayerRepository
    {
        public GamePlayerRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<GamePlayer>> GetGamePlayersForGameAsync(int gameId)
        {
            return await _context.GamePlayers
                .Include(gp => gp.Player)
                .Include(gp => gp.Statistics)
                .Where(gp => gp.GameId == gameId)
                .OrderBy(gp => gp.TurnOrder)
                .ToListAsync();
        }

        public async Task<GamePlayer> GetGamePlayerWithStatsAsync(int gamePlayerId)
        {
            return await _context.GamePlayers
                .Include(gp => gp.Player)
                .Include(gp => gp.Statistics)
                .FirstOrDefaultAsync(gp => gp.Id == gamePlayerId);
        }
    }
}

