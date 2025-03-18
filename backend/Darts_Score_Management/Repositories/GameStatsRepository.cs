using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class GameStatsRepository : BaseRepository<GameStats>, IGameStatsRepository
    {
        public GameStatsRepository(AppDbContext context) : base(context) { }

        public async Task<GameStats> GetByGameAndPlayerAsync(int gameId, int gamePlayerId)
        {
            return await _context.GameStats
                .FirstOrDefaultAsync(gs => gs.GameId == gameId && gs.GamePlayerId == gamePlayerId);
        }

        public async Task<IEnumerable<GameStats>> GetPlayerGameStatsAsync(int gamePlayerId)
        {
            return await _context.GameStats
                .Where(gs => gs.GamePlayerId == gamePlayerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<GameStats>> GetGameStatsForPlayerAsync(int playerId)
        {
            return await _context.GameStats
                .Include(gs => gs.GamePlayer).ThenInclude(gp => gp.Player)
                .Where(gs => gs.GamePlayer.PlayerId == playerId)
                .ToListAsync();
        }
    }
}
