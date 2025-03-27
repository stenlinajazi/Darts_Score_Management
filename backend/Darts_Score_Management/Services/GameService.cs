using AutoMapper;
using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game.Core;
using Darts_Score_Management.DTOs.Game.Response;
using Darts_Score_Management.DTOs.Game.State;
using Darts_Score_Management.DTOs.Game.Statistics;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Enums;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Darts_Score_Management.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;
        private readonly ISetService _setService;
        private readonly ILegService _legService;
        private readonly ITurnService _turnService;
        private readonly IPlayerRepository _playerRepository;

        public GameService(IGameRepository gameRepository, IMapper mapper, ISetService setService,
            ILegService legService, ITurnService turnService, IPlayerRepository playerRepository)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _setService = setService;
            _legService = legService;
            _turnService = turnService;
            _playerRepository = playerRepository;
        }
         public async Task<IEnumerable<GameListResponseDTO>> GetAllSummariesAsync()
         {
            return await _gameRepository.GetAllSummariesAsync();
        }

        public async Task<GameDTO> GetGameByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Game ID must be a positive number.", nameof(id));

            Game game = await _gameRepository.GetGameWithDetailsAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with ID {id} not found");

            return _mapper.Map<GameDTO>(game);
        }

        public async Task<GameDetailsResponseDTO> GetGameWithDetailsAndHistoryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Game ID must be a positive number.", nameof(id));

            return await _gameRepository.GetGameWithDetailsAndHistoryAsync(id);
        }

        public async Task<IEnumerable<PlayerGameSummaryDTO>> GetPlayerGamesAsync(int playerId)
        {
            if (playerId <= 0)
                throw new ArgumentException("Player ID must be a positive number.", nameof(playerId));
            var games = await _gameRepository.GetPlayerGamesAsync(playerId);
            if (games == null || !games.Any())
                throw new KeyNotFoundException($"No games found for player with ID {playerId}");
            return games;
        }

        public async Task<GameDTO> CreateGameAsync(CreateGameDTO createGameDto)
        {
            if (createGameDto == null) throw new ArgumentNullException(nameof(createGameDto));
 
            if (createGameDto.StartingScore != 301 && createGameDto.StartingScore != 501 && createGameDto.StartingScore != 701)
                throw new ValidationException("Starting score must be 301, 501, or 701");
            if (!Enum.IsDefined(typeof(GameType), createGameDto.Type))
                throw new ValidationException("Game type must be a valid GameType (X01 or Cricket).");

            if (createGameDto.PlayerIds == null || !createGameDto.PlayerIds.Any())
                throw new ValidationException("At least one player ID must be provided.");
            var invalidPlayerIds = new List<int>();
            foreach (var playerId in createGameDto.PlayerIds)
            {
                if (playerId <= 0)
                    throw new ValidationException($"Player ID {playerId} must be a positive number.");
                var player = await _playerRepository.GetByIdAsync(playerId);
                if (player == null)
                    invalidPlayerIds.Add(playerId);
            }
            if (invalidPlayerIds.Any())
                throw new ValidationException($"One or more player IDs do not exist: {string.Join(", ", invalidPlayerIds)}.");

            Game game = _mapper.Map<Game>(createGameDto);
            game.StartedAt = DateTime.UtcNow;
            game.IsComplete = false;

            if (game.Settings == null)
            {
                game.Settings = new GameSettings
                {
                    MustFinishOnDouble = createGameDto.Settings.MustFinishOnDouble,
                    SetsToWin = createGameDto.Settings.SetsToWin,
                    LegsPerSet = createGameDto.Settings.LegsPerSet
                };
            }

            List<GamePlayer> gamePlayers = createGameDto.PlayerIds.Select((playerId, index) => new GamePlayer
            {
                PlayerId = playerId,
                TurnOrder = index + 1 
            }).ToList();

            Game createdGame = await _gameRepository.CreateGameWithPlayersAsync(game, gamePlayers);
            SetDTO firstSet = await CreateNextSetAsync(createdGame.Id);
            return await GetGameByIdAsync(createdGame.Id);
        }

        public async Task DeleteGameAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Game ID must be a positive number.", nameof(id));
            var game = await _gameRepository.GetByIdAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with ID {id} not found");

            await _gameRepository.DeleteAsync(id);
        }

        public async Task<GameDTO> EndGameAsync(int id, int winnerId)
        {
            if (id <= 0)
                throw new ArgumentException("Game ID must be a positive number.", nameof(id));

            if (winnerId <= 0)
                throw new ArgumentException("Winner ID must be a positive number.", nameof(winnerId));
            Game game = await _gameRepository.GetGameWithDetailsAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with id {id} not found");

            game.IsComplete = true;
            game.EndedAt = DateTime.UtcNow;

            // Set winner and rankings
            List<GamePlayer> gamePlayers = game.GamePlayers.ToList();
            GamePlayer winner = gamePlayers.FirstOrDefault(gp => gp.PlayerId == winnerId);
            if (winner != null)
            {
                winner.IsWinner = true;
                winner.FinalRanking = 1;
            }


            Leg latestLeg = game.Sets
                .OrderByDescending(s => s.SetNumber)
                .SelectMany(s => s.Legs)
                .OrderByDescending(l => l.LegNumber)
                .FirstOrDefault();

            if (latestLeg == null)
                throw new InvalidOperationException("No legs found in the game");

            // Fetch ending scores for all players from their last turn in the latest leg
            Dictionary<int, int> playerScores = new Dictionary<int, int>();
            foreach (GamePlayer player in gamePlayers)
            {
                TurnDTO lastTurnDto = await _turnService.GetLastTurnByPlayerAndLegAsync(player.PlayerId, latestLeg.Id);
                int endingScore = lastTurnDto?.EndingScore ?? game.StartingScore;
                playerScores[player.PlayerId] = endingScore;
            }


            int ranking = 2;
            foreach (var player in gamePlayers
                .Where(gp => gp.PlayerId != winnerId)
                .OrderBy(gp => playerScores[gp.PlayerId]))
            {
                player.FinalRanking = ranking++;
            }

            await _gameRepository.UpdateAsync(game);
            return _mapper.Map<GameDTO>(game);
        }

        public async Task<SetDTO> CreateNextSetAsync(int gameId)
        {
            GameDTO game = await GetGameByIdAsync(gameId);
            if (game == null)
                throw new KeyNotFoundException($"Game with ID {gameId} not found");

            int nextSetNumber = game.Sets.Count + 1;
            CreateSetDTO createSetDto = new CreateSetDTO
            {
                GameId = gameId,
                SetNumber = nextSetNumber
            };
            SetDTO setDto = await _setService.CreateSetAsync(createSetDto);
            await CreateNextLegAsync(setDto.Id);
            return setDto;
        }

        public async Task<LegDTO> CreateNextLegAsync(int setId)
        {
            SetDTO set = await _setService.GetSetByIdAsync(setId);
            if (set == null)
                throw new KeyNotFoundException($"Set with ID {setId} not found");

            int nextLegNumber = set.Legs.Count + 1;
            CreateLegDTO createLegDto = new CreateLegDTO
            {
                SetId = set.Id,
                LegNumber = nextLegNumber
            };
            return await _legService.CreateLegAsync(createLegDto);
        }


        public async Task<int> GetActiveLegIdAsync()
        {
            var legId = await _gameRepository.GetActiveLegIdForMostRecentGameAsync();
            if (legId == null)
                throw new InvalidOperationException("No active leg found. Please create a new game or complete existing sets/legs.");
            return legId.Value;
        }

        public async Task<int> GetActiveLegIdByGameIdAsync(int gameId)
        {
            var game = await GetGameByIdAsync(gameId);
            if (game == null)
                throw new KeyNotFoundException($"Game with ID {gameId} not found");

            if (game.IsComplete)
                throw new InvalidOperationException($"Game with ID {gameId} is already complete.");

            var activeLeg = game.Sets
                .Where(s => !s.WinnerPlayerId.HasValue)
                .OrderBy(s => s.SetNumber)
                .SelectMany(s => s.Legs)
                .Where(l => !l.WinnerPlayerId.HasValue)
                .OrderBy(l => l.LegNumber)
                .FirstOrDefault();

            if (activeLeg == null)
                throw new InvalidOperationException($"No active leg found in game with ID {gameId}.");

            return activeLeg.Id;
        }

        public async Task<ResumeGameStateDTO> GetResumeGameStateAsync(int gameId)
        {
            if (gameId <= 0)
                throw new ArgumentException("Game ID must be a positive number.", nameof(gameId));

            var gameData = await _gameRepository.GetResumeGameDataAsync(gameId);

            var game = gameData.Game;
            var activeLeg = gameData.ActiveLeg;
            var lastTurn = gameData.LastTurn;

            var players = game.GamePlayers
                .OrderBy(gp => gp.TurnOrder)
                .Select(async gp =>
                {
                    var lastPlayerTurnDto = await _turnService.GetLastTurnByPlayerAndLegAsync(gp.PlayerId, activeLeg.Id);
                    var lastPlayerTurn = _mapper.Map<Turn>(lastPlayerTurnDto);
                    return new ResumePlayerDTO
                    {
                        Id = gp.PlayerId,
                        Name = gp.Player.Name,
                        StartingScore = lastPlayerTurn?.EndingScore ?? game.StartingScore,
                        RemainingScore = lastPlayerTurn?.EndingScore ?? game.StartingScore,
                        PointsThisTurn = lastTurn?.PlayerId == gp.PlayerId ? lastTurn.TotalPoints : 0
                    };
                })
                .Select(t => t.Result)
                .ToList();

            
            int activePlayerIndex = lastTurn == null
                ? 0
                : (game.GamePlayers.OrderBy(gp => gp.TurnOrder).ToList().FindIndex(gp => gp.PlayerId == lastTurn.PlayerId) + 1) % players.Count;

            var currentThrows = lastTurn?.Throws
                .Where(t => !t.IsBusted)
                .Select(t => new CreateThrowDTO { Segment = t.Segment, Multiplier = t.Multiplier })
                .ToList() ?? new List<CreateThrowDTO>();

            return new ResumeGameStateDTO
            {
                GameId = gameId,
                StartingScore = game.StartingScore,
                Players = players,
                ActivePlayerIndex = activePlayerIndex,
                CurrentThrows = currentThrows,
                Message = "Game resumed successfully"
            };
        }

    }
}
