using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class GameRepository : BaseRepository<Game>, IGameRepository
    {
        public GameRepository(AppDbContext context) : base(context) { }
        //public async Task<IEnumerable<Game>> GetAllSummariesAsync()
        //{
        //    return await _context.Games
        //         .Select(g => new Game
        //         {
        //               Id = g.Id,
        //               Type = g.Type,
        //               StartingScore = g.StartingScore,
        //               StartedAt = g.StartedAt,
        //               EndedAt = g.EndedAt,
        //               IsComplete = g.IsComplete,
        //               GamePlayers = g.GamePlayers, 
        //               Sets = g.Sets        
        //         })
        //         .ToListAsync();
        //}


        public async Task<IEnumerable<GameListResponseDTO>> GetAllSummariesAsync()
        {
            return await _context.Games
                .Select(g => new GameListResponseDTO
                {
                    Id = g.Id,
                    Type = g.Type.ToString(),
                    StartingScore = g.StartingScore,
                    SetsToWin = g.Settings.SetsToWin,
                    StartedAt = g.StartedAt,
                    EndedAt = g.EndedAt,
                    IsComplete = g.IsComplete,
                    WinnerId = g.GamePlayers.FirstOrDefault(gp => gp.IsWinner).PlayerId,
                    Players = g.GamePlayers.Select(gp => new GamePlayerInfoDTO
                    {
                        PlayerId = gp.PlayerId,
                        PlayerName = gp.Player.Name,
                        SetsWon = g.Sets.Count(s => s.WinnerPlayerId == gp.PlayerId)
                    }).ToList()
                })
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


        public async Task<GameDetailsResponseDTO> GetGameWithDetailsAndHistoryAsync(int id)
        {
            // Step 1: Load the game with its related entities
            var game = await _context.Games
                .Include(g => g.GamePlayers)
                    .ThenInclude(gp => gp.Player)
                .Include(g => g.Sets)
                    .ThenInclude(s => s.Legs)
                        .ThenInclude(l => l.Turns)
                            .ThenInclude(t => t.Throws)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                throw new KeyNotFoundException($"Game with ID {id} not found.");

            // Batch fetch GameStats, SetStats, and LegStats for all GamePlayers in the game
            var gamePlayerIds = game.GamePlayers.Select(gp => gp.Id).ToList();
            var gameStats = await _context.GameStats
                .Where(gs => gs.GameId == id && gamePlayerIds.Contains(gs.GamePlayerId))
                .ToListAsync();
            var setStats = await _context.SetStats
                .Where(ss => game.Sets.Select(s => s.Id).Contains(ss.SetId) && gamePlayerIds.Contains(ss.GamePlayerId))
                .ToListAsync();
            var legIds = game.Sets.SelectMany(s => s.Legs).Select(l => l.Id).ToList();
            var legStats = await _context.LegStats
                .Include(ls => ls.Leg)
                .ThenInclude(l => l.Turns)
                .ThenInclude(t => t.Throws)
                .Where(ls => legIds.Contains(ls.LegId) && gamePlayerIds.Contains(ls.GamePlayerId))
                .ToListAsync();

            // Group stats by GamePlayerId for easy mapping
            var gameStatsDictionary = gameStats.ToDictionary(gs => gs.GamePlayerId, gs => gs);
            var setStatsDictionary = setStats
                .GroupBy(ss => ss.GamePlayerId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var legStatsDictionary = legStats
                .GroupBy(ls => ls.GamePlayerId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Build the response
            var response = new GameDetailsResponseDTO
            {
                Id = game.Id,
                Type = game.Type.ToString(),
                StartingScore = game.StartingScore,
                SetsToWin = game.Settings?.SetsToWin ?? 0, 
                StartedAt = game.StartedAt,
                EndedAt = game.EndedAt,
                IsComplete = game.IsComplete,
                WinnerId = game.GamePlayers.FirstOrDefault(gp => gp.IsWinner)?.PlayerId,
                Players = game.GamePlayers.Select(gp => new PlayerMatchStatsDTO
                {
                    PlayerId = gp.PlayerId,
                    PlayerName = gp.Player?.Name,
                    GameStats = gameStatsDictionary.TryGetValue(gp.Id, out var gs)
                        ? new GameStatsDTO
                        {
                            SetsWin = gs.SetsWin,
                            LegsWin = gs.LegsWin,
                            PPD = (decimal)gs.PPD,
                            First9PPD = (decimal)gs.First9PPD,
                            CheckoutPercentage = gs.CheckoutPercentage.ToString(),
                            Count60Plus = gs.Count60Plus,
                            Count100Plus = gs.Count100Plus,
                            Count140Plus = gs.Count140Plus,
                            Count180s = gs.Count180s
                        }
                        : null,
                    SetStats = setStatsDictionary.TryGetValue(gp.Id, out var ssList)
                        ? ssList.Select(ss => new SetStatsDTO
                        {
                            SetId = ss.SetId,
                            LegsWin = ss.LegsWin,
                            PPD = (decimal)ss.PPD,
                            First9PPD = (decimal)ss.First9PPD,
                            CheckoutPercentage = ss.CheckoutPercentage.ToString(),
                            Count60Plus = ss.Count60Plus,
                            Count100Plus = ss.Count100Plus,
                            Count140Plus = ss.Count140Plus,
                            Count180s = ss.Count180s
                        }).ToList()
                        : new List<SetStatsDTO>(),
                    LegStats = legStatsDictionary.TryGetValue(gp.Id, out var lsList)
                        ? lsList.Select(ls => new LegStatsDTO
                        {
                            LegId = ls.LegId,
                            PPD = (decimal)ls.PPD,
                            First9PPD = (decimal)ls.First9PPD,
                            TotalThrows = ls.TotalThrows,
                            CheckoutPercentage = ls.CheckoutPercentage.ToString(),
                            Count60Plus = ls.Count60Plus,
                            Count100Plus = ls.Count100Plus,
                            Count140Plus = ls.Count140Plus,
                            Count180s = ls.Count180s,
                            History = ls.Leg.Turns
                                .Where(t => t.PlayerId == gp.PlayerId)
                                .Select(t => new TurnHistoryDTO
                                {
                                    TurnId = t.Id,
                                    EndingScore = t.EndingScore,
                                    Throws = t.Throws.Select(th => new CreateThrowDTO
                                    {
                                        Segment = th.Segment,
                                        Multiplier = th.Multiplier
                                    }).ToList()
                                }).ToList()
                        }).ToList()
                        : new List<LegStatsDTO>()
                }).ToList()
            };

            return response;
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

        

    }
}
