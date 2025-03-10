using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
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

        public async Task<IEnumerable<PlayerGameSummaryDTO>> GetPlayerGamesAsync(int playerId)
        {
            return await _context.Games
                 .Where(g => g.GamePlayers.Any(gp => gp.PlayerId == playerId))
                 .Select(g => new
                 {
                     Game = g,
                     GamePlayer = g.GamePlayers.FirstOrDefault(gp => gp.PlayerId == playerId),
                     GameStats = _context.GameStats.FirstOrDefault(gs =>
                         gs.GameId == g.Id &&
                         gs.GamePlayerId == g.GamePlayers.FirstOrDefault(gp => gp.PlayerId == playerId).Id)
                 })
                 .Select(x => new PlayerGameSummaryDTO
                 {
                     GameId = x.Game.Id,
                     Type = x.Game.Type,
                     StartingScore = x.Game.StartingScore,
                     StartedAt = x.Game.StartedAt,
                     EndedAt = x.Game.EndedAt,
                     IsComplete = x.Game.IsComplete,
                     PlayerCount = x.Game.GamePlayers.Count(),
                     SetsCount = x.Game.Sets.Count(),
                     WasWinner = x.GamePlayer.IsWinner,
                     FinalRanking = x.GamePlayer.FinalRanking,
                     SetsWon = x.GameStats != null
                         ? x.GameStats.SetsWin
                         : x.Game.Sets.Count(s => s.WinnerPlayerId == playerId),
                     LegsWon = x.GameStats != null
                         ? x.GameStats.LegsWin
                         : x.Game.Sets.SelectMany(s => s.Legs).Count(l => l.WinnerPlayerId == playerId)
                 })
                 .ToListAsync();
        }


        public async Task<Game> CreateGameWithPlayersAsync(Game game, IEnumerable<GamePlayer> gamePlayers)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Add and save the Game first to generate its Id
                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                // Set the GameId for each GamePlayer and add them
                foreach (var gamePlayer in gamePlayers)
                {
                    gamePlayer.GameId = game.Id; // Assign the generated Game.Id
                }

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

        public async Task<IEnumerable<Game>> GetAllSummariesAsync()
        {
            return await _context.Games
                .Select(g => new Game
                {
                    Id = g.Id,
                    Type = g.Type,
                    StartingScore = g.StartingScore,
                    StartedAt = g.StartedAt,
                    EndedAt = g.EndedAt,
                    IsComplete = g.IsComplete,
                    GamePlayers = g.GamePlayers, 
                    Sets = g.Sets        
                })
                .ToListAsync();
        }

    }
}
