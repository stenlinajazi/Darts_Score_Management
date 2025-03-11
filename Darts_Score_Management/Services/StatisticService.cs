using AutoMapper;
using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.Enums;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly ILegStatsRepository _legStatsRepository;
        private readonly ISetStatsRepository _setStatsRepository;
        private readonly IGameStatsRepository _gameStatsRepository;
        private readonly ITurnService _turnService;
        private readonly ILegService _legService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
      

        public StatisticService(ILegStatsRepository legStatsRepository,
            ISetStatsRepository setStatsRepository,
            IGameStatsRepository gameStatsRepository,
            ITurnService turnService,
            IMapper mapper,
            AppDbContext context,
            ILegService legService)
        {
            _legStatsRepository = legStatsRepository;
            _setStatsRepository = setStatsRepository;
            _gameStatsRepository = gameStatsRepository;
            _turnService = turnService;
            _mapper = mapper;
            _context = context;
            _legService = legService;
        }

        public async Task UpdateLegStatsAsync(int legId, List<GamePlayer> gamePlayers)
        {
            var leg = await _legService.GetLegByIdAsync(legId);
            if (leg == null || leg.Set == null || leg.Set.Game == null)
                throw new KeyNotFoundException($"Leg with ID {legId} or its related entities not found.");

            foreach (var gamePlayer in gamePlayers)
            {
                var currentPlayerId = gamePlayer.PlayerId;
                var turns = await _turnService.GetPlayerTurnsInLegAsync(currentPlayerId, legId);
                var legStats = CalculateLegStats(turns, gamePlayer.Id, legId);
                await _legStatsRepository.AddAsync(legStats);
            }
        }

        public async Task UpdateSetStatsAsync(int setId, Dictionary<int, int> legsWonPerPlayer)
        {
            var gamePlayers = await GetGamePlayersForSetAsync(setId);
   
            foreach (var gp in gamePlayers)
            {
                var playerLegStats = await _legStatsRepository.GetLegStatsForPlayerInSetAsync(setId, gp.Id, gp.PlayerId);
                var setStats = AggregateSetStats(playerLegStats, gp.Id, setId, legsWonPerPlayer);
                await _setStatsRepository.AddAsync(setStats);
            }
        }

        public async Task UpdateGameStatsAsync(int gameId, Dictionary<int, int> setsWonPerPlayer)
        {
            var gamePlayers = await GetGamePlayersForGameAsync(gameId);
           
            foreach (var gp in gamePlayers)
            {
                var playerSetStats = await _setStatsRepository.GetSetStatsForPlayerInGameAsync(gameId, gp.Id, gp.PlayerId);
                var gameStats = AggregateGameStats(playerSetStats, gp.Id, gameId, setsWonPerPlayer);
                await _gameStatsRepository.AddAsync(gameStats);
            }
        }

        public async Task<LegStatsDTO> GetLegStatsAsync(int legId, int gamePlayerId)
        {
            var legStats = await _legStatsRepository.GetByLegAndPlayerAsync(legId, gamePlayerId);
            if (legStats == null) throw new KeyNotFoundException($"Leg stats for LegId {legId} and GamePlayerId {gamePlayerId} not found.");
            return _mapper.Map<LegStatsDTO>(legStats);
        }

        public async Task<SetStatsDTO> GetSetStatsAsync(int setId, int gamePlayerId)
        {
            var setStats = await _setStatsRepository.GetBySetAndPlayerAsync(setId, gamePlayerId);
            if (setStats == null) throw new KeyNotFoundException($"Set stats for SetId {setId} and GamePlayerId {gamePlayerId} not found.");
            return _mapper.Map<SetStatsDTO>(setStats);
        }

        public async Task<GameStatsDTO> GetGameStatsAsync(int gameId, int gamePlayerId)
        {
            var gameStats = await _gameStatsRepository.GetByGameAndPlayerAsync(gameId, gamePlayerId);
            if (gameStats == null) throw new KeyNotFoundException($"Game stats for GameId {gameId} and GamePlayerId {gamePlayerId} not found.");
            return _mapper.Map<GameStatsDTO>(gameStats);
        }

        public async Task<PlayerStatsDTO> GetPlayerStatsAsync(int playerId)
        {
            var gamePlayers = await GetPlayerGamePlayersAsync(playerId);
            var last10LegStats = await _legStatsRepository.GetLast10LegsForPlayerAsync(playerId);
            return CalculatePlayerStats(last10LegStats, gamePlayers);
        }

      

        private async Task<List<GamePlayer>> GetGamePlayersForSetAsync(int setId)
        {
            var set = await _context.Sets
                .Include(s => s.Game.GamePlayers)
                .FirstOrDefaultAsync(s => s.Id == setId);
            if (set == null) throw new KeyNotFoundException($"Set with ID {setId} not found.");
            var gamePlayers = set.Game.GamePlayers.ToList();
            Console.WriteLine($"GamePlayers for Set {setId}:");
            foreach (var gp in gamePlayers)
            {
                Console.WriteLine($"GamePlayerId = {gp.Id}, PlayerId = {gp.PlayerId}, TurnOrder = {gp.TurnOrder}");
            }
            return gamePlayers;
        }

        private async Task<List<GamePlayer>> GetGamePlayersForGameAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.GamePlayers)
                .FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null) throw new KeyNotFoundException($"Game with ID {gameId} not found.");
            return game.GamePlayers.ToList();
        }

        private async Task<List<GamePlayer>> GetPlayerGamePlayersAsync(int playerId)
        {
            return await _context.GamePlayers
                .Include(gp => gp.Player)
                .Include(gp => gp.LegStats)
                .Where(gp => gp.PlayerId == playerId)
                .ToListAsync();
        }

        private LegStats CalculateLegStats(IEnumerable<Turn> turns, int gamePlayerId, int legId)
        {
            var ppd = StatisticCalculator.CalculatePPD(turns);
            var first9PPD = StatisticCalculator.CalculateFirst9PPD(turns);
            var totalThrows = turns.Sum(t => t.Throws.Count);
            var checkoutPercentage = StatisticCalculator.CalculateCheckoutPercentage(turns);
            var count60Plus = StatisticCalculator.CalculateCount60Plus(turns);
            var count100Plus = StatisticCalculator.CalculateCount100Plus(turns);
            var count140Plus = StatisticCalculator.CalculateCount140Plus(turns);
            var count180s = StatisticCalculator.CalculateCount180s(turns);
     
            return new LegStats
            {
                GamePlayerId = gamePlayerId,
                LegId = legId,
                PPD = ppd,
                First9PPD = first9PPD,
                TotalThrows = totalThrows,
                CheckoutPercentage = checkoutPercentage,
                Count60Plus = count60Plus,
                Count100Plus = count100Plus,
                Count140Plus = count140Plus,
                Count180s = count180s,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };
        }

        private SetStats AggregateSetStats(IEnumerable<LegStats> legStats, int gamePlayerId, int setId, Dictionary<int, int> legsPerPlayer)
        {
            var totalThrows = legStats.Sum(ls => ls.TotalThrows);
            var ppd = StatisticCalculator.CalculatePPDAggregate(legStats);
            var first9PPD = StatisticCalculator.CalculateFirst9PPDAggregate(legStats);
            var gamePlayer = _context.GamePlayers.FirstOrDefault(gp => gp.Id == gamePlayerId);
            var playerId = gamePlayer?.PlayerId ?? 0;
            var legsWon = legsPerPlayer.ContainsKey(playerId) ? legsPerPlayer[playerId] : 0;
            var playerTurns = legStats.SelectMany(ls => _context.Turns
            .Where(t => t.LegId == ls.LegId && t.PlayerId == playerId));
            var checkoutPercentage = StatisticCalculator.CalculateCheckoutPercentage(playerTurns);
            var count60Plus = legStats.Sum(ls => ls.Count60Plus);
            var count100Plus = legStats.Sum(ls => ls.Count100Plus);
            var count140Plus = legStats.Sum(ls => ls.Count140Plus);
            var count180s = legStats.Sum(ls => ls.Count180s);

            return new SetStats
            {
                GamePlayerId = gamePlayerId,
                SetId = setId,
                LegsWin = legsWon,
                PPD = ppd,
                First9PPD = first9PPD,
                TotalThrows = totalThrows,
                CheckoutPercentage = checkoutPercentage,
                Count60Plus = count60Plus,
                Count100Plus = count100Plus,
                Count140Plus = count140Plus,
                Count180s = count180s,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };
        }

        private GameStats AggregateGameStats(IEnumerable<SetStats> setStats, int gamePlayerId, int gameId, Dictionary<int, int> setsWonPerPlayer)
        {
            var totalThrows = setStats.Sum(ss => ss.TotalThrows);
            var ppd = StatisticCalculator.CalculatePPDAggregate(setStats);
            var first9PPD = StatisticCalculator.CalculateFirst9PPDAggregate(setStats);
            var gamePlayer = _context.GamePlayers.FirstOrDefault(gp => gp.Id == gamePlayerId);
            var playerId = gamePlayer?.PlayerId ?? 0;
            var setsWon = setsWonPerPlayer.ContainsKey(playerId) ? setsWonPerPlayer[playerId] : 0;
            var legsWon = setStats.Sum(ss => ss.LegsWin);
            var playerTurns = setStats.SelectMany(ss => _context.Turns
            .Where(t => t.Leg.SetId == ss.SetId && t.PlayerId == playerId));
            var checkoutPercentage = StatisticCalculator.CalculateCheckoutPercentage(playerTurns);
            var count60Plus = setStats.Sum(ss => ss.Count60Plus);
            var count100Plus = setStats.Sum(ss => ss.Count100Plus);
            var count140Plus = setStats.Sum(ss => ss.Count140Plus);
            var count180s = setStats.Sum(ss => ss.Count180s);

            return new GameStats
            {
                GamePlayerId = gamePlayerId,
                GameId = gameId,
                SetsWin = setsWon,
                LegsWin = legsWon,
                PPD = ppd,
                First9PPD = first9PPD,
                CheckoutPercentage = checkoutPercentage,
                Count60Plus = count60Plus,
                Count100Plus = count100Plus,
                Count140Plus = count140Plus,
                Count180s = count180s,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };
        }

        private PlayerStatsDTO CalculatePlayerStats(IEnumerable<LegStats> last10LegStats, IEnumerable<GamePlayer> gamePlayers)
        {
            if (!gamePlayers.Any())
                throw new NullReferenceException("No game players found for the given player ID.");
            var playerId = gamePlayers.First().PlayerId;
            var playerName = gamePlayers.First().Player?.Name ?? throw new NullReferenceException("Player name is null.");
         
            var allLegStats = gamePlayers.SelectMany(gp => gp.LegStats ?? Enumerable.Empty<LegStats>()).ToList();
            var allLegStatsCombined = allLegStats.Concat(last10LegStats)
                .DistinctBy(ls => ls.Id) 
                .ToList();
            var totalLegsPlayed = allLegStatsCombined.Count;
            var legsWon = allLegStatsCombined.Count(ls => ls.Leg?.WinnerPlayerId == playerId);
            var legsLost = totalLegsPlayed - legsWon;

            var last10Stats = new Last10StatsDTO();
            var allStats = new AllStatsDTO();

          
            if (last10LegStats.Any())
            {
                last10Stats.PPD = new StatSummary
                {
                    Average = Math.Round(last10LegStats.Average(ls => ls.PPD), 2),
                    Best = Math.Round(last10LegStats.Max(ls => ls.PPD), 2)
                };
                last10Stats.First9PPD = new StatSummary
                {
                    Average = Math.Round(last10LegStats.Average(ls => ls.First9PPD), 2),
                    Best = Math.Round(last10LegStats.Max(ls => ls.First9PPD), 2)
                };

                var last10Turns = last10LegStats.SelectMany(ls => _context.Turns
                    .Where(t => t.LegId == ls.LegId && t.PlayerId == playerId));

                last10Stats.CheckoutPercentage = new StatSummary
                {
                    Average = StatisticCalculator.CalculateCheckoutPercentage(last10Turns),
                    Best = Math.Round((decimal)last10LegStats.Max(ls => ls.CheckoutPercentage), 2)
                };

                var winsInLast10 = last10LegStats.Count(ls => ls.Leg?.WinnerPlayerId == playerId);
                last10Stats.WinPercentage = new StatSummary
                {
                    Average = Math.Round((decimal)winsInLast10 / last10LegStats.Count() * 100, 1),
                    Best = Math.Round((decimal)last10LegStats.Count(ls => ls.Leg?.WinnerPlayerId == playerId) / last10LegStats.Count() * 100, 1)
                };
            }

            // Calculate totals for all stats

            allStats.Count60Plus = new StatTotals
            {
                Total = allLegStatsCombined.Any() ? allLegStatsCombined.Sum(ls => ls.Count60Plus) : 0,
                PerLeg = allLegStatsCombined.Any() ? Math.Round((decimal)allLegStatsCombined.Sum(ls => ls.Count60Plus) / allLegStatsCombined.Count, 1) : 0
            };
            allStats.Count100Plus = new StatTotals
            {
                Total = allLegStatsCombined.Any() ? allLegStatsCombined.Sum(ls => ls.Count100Plus) : 0,
                PerLeg = allLegStatsCombined.Any() ? Math.Round((decimal)allLegStatsCombined.Sum(ls => ls.Count100Plus) / allLegStatsCombined.Count, 1) : 0
            };

            allStats.Count140Plus = new StatTotals
            {
                Total = allLegStatsCombined.Any() ? allLegStatsCombined.Sum(ls => ls.Count140Plus) : 0,
                PerLeg = allLegStatsCombined.Any() ? Math.Round((decimal)allLegStatsCombined.Sum(ls => ls.Count140Plus) / allLegStatsCombined.Count, 1) : 0
            };

            allStats.Count180s = new StatTotals
            {
                Total = allLegStatsCombined.Any() ? allLegStatsCombined.Sum(ls => ls.Count180s) : 0,
                PerLeg = allLegStatsCombined.Any() ? Math.Round((decimal)allLegStatsCombined.Sum(ls => ls.Count180s) / allLegStatsCombined.Count, 1) : 0
            };

            return new PlayerStatsDTO
                {
                PlayerId = playerId,
                PlayerName = playerName,
                TotalLegsPlayed = totalLegsPlayed,
                LegsWon = $"{legsWon}/{totalLegsPlayed}",
                Last10LegsStats = last10Stats,
                AllStats = allStats
            };
            }
       
        }
    }

