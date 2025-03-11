using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Set;
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
        public async Task<IEnumerable<GameDTO>> GetAllGamesAsync()
        {
           var games = await _gameRepository.GetAllAsync();
           return _mapper.Map<IEnumerable<GameDTO>>(games);
        }
        public async Task<GameDTO> GetGameByIdAsync(int id)
        {
            var game = await _gameRepository.GetGameWithDetailsAsync(id);
            return _mapper.Map<GameDTO>(game);
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

            var game = _mapper.Map<Game>(createGameDto);
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

            //game.DeletedBy = string.Empty;
            //game.ModifiedBy = string.Empty;

            var gamePlayers = createGameDto.PlayerIds.Select((playerId, index) => new GamePlayer
            {
                PlayerId = playerId,
                TurnOrder = index + 1 // Initialize TurnOrder based on the order of PlayerIds in the list
            }).ToList();

            var createdGame = await _gameRepository.CreateGameWithPlayersAsync(game, gamePlayers);
            // Automatically create sets and legs for a best-of game
            await CreateSetsAndLegsForGame(createdGame, createGameDto.Settings.SetsToWin, createGameDto.Settings.LegsPerSet);
            return await GetGameByIdAsync(createdGame.Id);
        }

        public async Task<GameDTO> UpdateGameAsync(int id, GameDTO gameDto)
        {
            var game = await _gameRepository.GetByIdAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with id {id} not found");

            _mapper.Map(gameDto, game);
            await _gameRepository.UpdateAsync(game);
            return _mapper.Map<GameDTO>(game);
        }

        public async Task DeleteGameAsync(int id)
        {
            await _gameRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PlayerGameSummaryDTO>> GetPlayerGamesAsync(int playerId)
        {
            return await _gameRepository.GetPlayerGamesAsync(playerId);
        }
        

        public async Task<GameDTO> EndGameAsync(int id, int winnerId)
        {
            var game = await _gameRepository.GetGameWithDetailsAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with id {id} not found");

            game.IsComplete = true;
            game.EndedAt = DateTime.UtcNow;

            // Set winner and rankings
            var gamePlayers = game.GamePlayers.ToList();
            var winner = gamePlayers.FirstOrDefault(gp => gp.PlayerId == winnerId);
            if (winner != null)
            {
                winner.IsWinner = true;
                winner.FinalRanking = 1;
            }

            // Get the latest leg in the game (last set, last leg)
            var latestLeg = game.Sets
                .OrderByDescending(s => s.SetNumber)
                .SelectMany(s => s.Legs)
                .OrderByDescending(l => l.LegNumber)
                .FirstOrDefault();

            if (latestLeg == null)
                throw new InvalidOperationException("No legs found in the game");

            // Fetch ending scores for all players from their last turn in the latest leg
            var playerScores = new Dictionary<int, int>();
            foreach (var player in gamePlayers)
            {
                var lastTurnDto = await _turnService.GetLastTurnByPlayerAndLegAsync(player.PlayerId, latestLeg.Id);
                int endingScore = lastTurnDto?.EndingScore ?? game.StartingScore;
                playerScores[player.PlayerId] = endingScore;
            }

            // Rank losers based on ending score (ascending: least points = higher rank)
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

        private async Task CreateSetsAndLegsForGame(Game game, int setsToWin, int legsPerSet)
        {
            
            for (int setNumber = 1; setNumber <= setsToWin; setNumber++)
            {
                var createSetDto = new CreateSetDTO
                {
                    GameId = game.Id,
                    SetNumber = setNumber
                };

                var setDto = await _setService.CreateSetAsync(createSetDto);

                for (int legNumber = 1; legNumber <= legsPerSet; legNumber++)
                {
                    var createLegDto = new CreateLegDTO
                    {
                        SetId = setDto.Id,
                        LegNumber = legNumber
                    };

                    await _legService.CreateLegAsync(createLegDto);
                }
            }
        }

        public async Task<IEnumerable<GameSummaryDTO>> GetAllGameSummariesAsync()
        {
            var games = await _gameRepository.GetAllSummariesAsync();
            return _mapper.Map<IEnumerable<GameSummaryDTO>>(games);
        }

    }
}
