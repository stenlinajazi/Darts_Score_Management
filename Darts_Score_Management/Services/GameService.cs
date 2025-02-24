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

            var game = _mapper.Map<Game>(createGameDto);
            game.StartedAt = DateTime.UtcNow;
            game.IsComplete = false;

            //game.DeletedBy = string.Empty;
            //game.ModifiedBy = string.Empty;

            var gamePlayers = createGameDto.PlayerIds.Select((playerId, index) => new GamePlayer
            {
                PlayerId = playerId,
                TurnOrder = index + 1
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

            // Set winner
            var winner = game.GamePlayers.FirstOrDefault(gp => gp.PlayerId == winnerId);
            if (winner != null)
            {
                winner.IsWinner = true;
            }

            await _gameRepository.UpdateAsync(game);
            return _mapper.Map<GameDTO>(game);
        }

        private async Task CreateSetsAndLegsForGame(Game game, int setsToWin, int legsPerSet)
        {
            // Calculate maximum sets needed for best-of (e.g., best of 2 sets requires 3 sets)
            int totalSets = setsToWin * 2 - 1;
            totalSets = Math.Max(1, totalSets); // Ensure at least 1 set

            for (int setNumber = 1; setNumber <= totalSets; setNumber++)
            {
                var createSetDto = new CreateSetDTO
                {
                    GameId = game.Id,
                    SetNumber = setNumber
                };

                var setDto = await _setService.CreateSetAsync(createSetDto);

                // Calculate maximum legs needed per set for best-of (e.g., best of 3 legs requires 5 legs)
                int totalLegs = legsPerSet * 2 - 1;
                totalLegs = Math.Max(1, totalLegs); // Ensure at least 1 leg

                for (int legNumber = 1; legNumber <= totalLegs; legNumber++)
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
