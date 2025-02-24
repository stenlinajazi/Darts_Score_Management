using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;

        public GameService(IGameRepository gameRepository, IMapper mapper)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task<GameDTO> GetGameByIdAsync(int id)
        {
            var game = await _gameRepository.GetGameWithDetailsAsync(id);
            return _mapper.Map<GameDTO>(game);
        }

        public async Task<IEnumerable<GameDTO>> GetAllGamesAsync()
        {
            var games = await _gameRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GameDTO>>(games);
        }

        public async Task<GameDTO> CreateGameAsync(CreateGameDTO createGameDto)
        {
            if (createGameDto == null) throw new ArgumentNullException(nameof(createGameDto));
            if (createGameDto.PlayerIds == null || !createGameDto.PlayerIds.Any())
                throw new ValidationException("At least one player is required");

            var game = new Game
            {
                Type = createGameDto.Type,
                StartingScore = createGameDto.StartingScore,
                StartedAt = DateTime.UtcNow,
                IsComplete = false,
                Settings = new GameSettings
                {
                    MustFinishOnDouble = createGameDto.Settings.MustFinishOnDouble,
                    SetsToWin = createGameDto.Settings.SetsToWin,
                    LegsPerSet = createGameDto.Settings.LegsPerSet
                }
            };

            //game.DeletedBy = string.Empty;
            //game.ModifiedBy = string.Empty;

            var gamePlayers = createGameDto.PlayerIds.Select((playerId, index) => new GamePlayer
            {
                PlayerId = playerId,
                TurnOrder = index + 1
            }).ToList();

            var createdGame = await _gameRepository.CreateGameWithPlayersAsync(game, gamePlayers);
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

    }
}
