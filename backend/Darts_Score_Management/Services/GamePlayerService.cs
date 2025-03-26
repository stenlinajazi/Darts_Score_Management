using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class GamePlayerService : IGamePlayerService
    {
        private readonly IGamePlayerRepository _gamePlayerRepository;
       
        public GamePlayerService(IGamePlayerRepository gamePlayerRepository)
        {
            _gamePlayerRepository = gamePlayerRepository;
        }

        public async Task<GamePlayerDTO> GetGamePlayerByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive number.", nameof(id));
            var gamePlayer = await _gamePlayerRepository.GetGamePlayerAsync(id);
            if (gamePlayer == null)
                throw new KeyNotFoundException($"Game player with ID {id} not found.");
            return gamePlayer;
        }

        public async Task<IEnumerable<GamePlayerDTO>> GetGamePlayersByGameIdAsync(int gameId)
        {
            if (gameId <= 0)
                throw new ArgumentException("Game ID must be a positive number.", nameof(gameId));
            var gamePlayers = await _gamePlayerRepository.GetGamePlayersForGameAsync(gameId);
            if (gamePlayers == null || !gamePlayers.Any())
                throw new KeyNotFoundException($"No game players found for game with ID {gameId}.");
            return gamePlayers;
        }
    }
}
