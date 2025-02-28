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

        public GameService(IGameRepository gameRepository, IMapper mapper, ISetService setService,
            ILegService legService)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _setService = setService;
            _legService = legService;
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

        public async Task<IEnumerable<GameDTO>> GetPlayerGamesAsync(int playerId)
        {
            var games = await _gameRepository.GetPlayerGamesAsync(playerId);
            return _mapper.Map<IEnumerable<GameDTO>>(games);
        }

        public async Task<GameDTO> EndGameAsync(int id, int winnerId)
        {
            var game = await _gameRepository.GetGameWithDetailsAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with id {id} not found");

            game.IsComplete = true;
            game.EndedAt = DateTime.UtcNow;

            // Set winner and rankings
            var gamePlayers = game.GamePlayers.OrderBy(gp => gp.TurnOrder).ToList();
            int ranking = 1;

            // Set winner's ranking (rank 1) and mark as winner
            var winner = gamePlayers.FirstOrDefault(gp => gp.PlayerId == winnerId);
            if (winner != null)
            {
                winner.IsWinner = true;
                winner.FinalRanking = ranking++;
            }

            // Set rankings for losers (rank 2, 3, etc.) based on TurnOrder
            foreach (var player in gamePlayers.Where(gp => gp.PlayerId != winnerId))
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

    }
}
