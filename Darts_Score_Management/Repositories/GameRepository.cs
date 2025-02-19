using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class GameRepository : BaseRepository<Game>, IGameRepository
    {
        public GameRepository(AppDbContext context) : base(context) { }

       
        public override async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games
                .Include(g => g.GamePlayers)
                    .ThenInclude(gp => gp.Player)
                .Include(g => g.Sets)
                    .ThenInclude(s => s.Legs)
                .ToListAsync();
        }

        public async Task<Game> GetGameWithDetailsAsync(int id)
        {
            return await _context.Games
                .Include(g => g.GamePlayers)
                    .ThenInclude(gp => gp.Player)
                .Include(g => g.Sets)
                    .ThenInclude(s => s.Legs)
                        .ThenInclude(l => l.Turns)
                            .ThenInclude(t => t.Throws)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Game>> GetPlayerGamesAsync(int playerId)
        {
            return await _context.Games
                .Include(g => g.GamePlayers)
                .Where(g => g.GamePlayers.Any(gp => gp.PlayerId == playerId))
                .ToListAsync();
        }

        //public async Task<Game> CreateGameAsync(Game game, List<int> playerIds)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        await _dbSet.AddAsync(game);
        //        await _context.SaveChangesAsync();

        //        var gamePlayers = playerIds.Select((playerId, index) => new GamePlayer
        //        {
        //            GameId = game.Id,
        //            PlayerId = playerId,
        //            TurnOrder = index + 1,
        //            //DeletedBy = string.Empty,   
        //            //ModifiedBy = string.Empty
        //        }).ToList();

        //        await _context.GamePlayers.AddRangeAsync(gamePlayers);
        //        await _context.SaveChangesAsync();

        //        await transaction.CommitAsync();
        //        return game;
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        public async Task<Game> CreateGameWithPlayersAsync(Game game, IEnumerable<GamePlayer> gamePlayers)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _dbSet.AddAsync(game);
                await _context.SaveChangesAsync();

                await _context.GamePlayers.AddRangeAsync(gamePlayers);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return game;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
