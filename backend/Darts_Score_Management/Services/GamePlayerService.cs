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
           return await _gamePlayerRepository.GetGamePlayerAsync(id);      
        }

        public async Task<IEnumerable<GamePlayerDTO>> GetGamePlayersByGameIdAsync(int gameId)
        {
            return await  _gamePlayerRepository.GetGamePlayersForGameAsync(gameId);
        }
    }
}
