using AutoMapper;
using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game.Core;
using Darts_Score_Management.DTOs.Game.Response;
using Darts_Score_Management.DTOs.Game.Statistics;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
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

        public GameService(IGameRepository gameRepository, IMapper mapper, ISetService setService,
            ILegService legService, ITurnService turnService)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _setService = setService;
            _legService = legService;
            _turnService = turnService;
        }
         public async Task<IEnumerable<GameListResponseDTO>> GetAllSummariesAsync()
         {
            return await _gameRepository.GetAllSummariesAsync();
        }

        public async Task<GameDTO> GetGameByIdAsync(int id)
        {
            Game game = await _gameRepository.GetGameWithDetailsAsync(id);
            return _mapper.Map<GameDTO>(game);
        }

        public async Task<GameDetailsResponseDTO> GetGameWithDetailsAndHistoryAsync(int id)
        {
            return await _gameRepository.GetGameWithDetailsAndHistoryAsync(id);
        }

        public async Task<IEnumerable<PlayerGameSummaryDTO>> GetPlayerGamesAsync(int playerId)
        {
            return await _gameRepository.GetPlayerGamesAsync(playerId);
        }

        public async Task<GameDTO> CreateGameAsync(CreateGameDTO createGameDto)
        {
            if (createGameDto == null) throw new ArgumentNullException(nameof(createGameDto));
            if (createGameDto.PlayerIds == null || !createGameDto.PlayerIds.Any())
                throw new ValidationException("At least one player is required");
            if (createGameDto.Settings == null)
                throw new ValidationException("Game settings are required");
            if (createGameDto.Settings.SetsToWin < 1)
                throw new ValidationException("SetsToWin must be at least 1");
            if (createGameDto.Settings.LegsPerSet < 1)
                throw new ValidationException("LegsPerSet must be at least 1");
            if (createGameDto.Settings.SetsToWin > 3)
                throw new ValidationException("SetsToWin cannot exceed 3");
            if (createGameDto.Settings.LegsPerSet > 3)
                throw new ValidationException("LegsPerSet cannot exceed 3");
            if (createGameDto.StartingScore != 301 && createGameDto.StartingScore != 501 && createGameDto.StartingScore != 701)
                throw new ValidationException("Starting score must be 301, 501, or 701");

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
                TurnOrder = index + 1 // Initialize TurnOrder based on the order of PlayerIds in the list
            }).ToList();

            Game createdGame = await _gameRepository.CreateGameWithPlayersAsync(game, gamePlayers);
            SetDTO firstSet = await CreateNextSetAsync(createdGame.Id);
            //await CreateNextLegAsync(firstSet.Id);
            return await GetGameByIdAsync(createdGame.Id);
        }

        public async Task DeleteGameAsync(int id)
        {
            await _gameRepository.DeleteAsync(id);
        }

        public async Task<GameDTO> EndGameAsync(int id, int winnerId)
        {
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

    }
}
