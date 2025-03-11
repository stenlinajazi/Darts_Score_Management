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
        private readonly IStatisticRepository _statisticRepository;
        private readonly IMapper _mapper;

        public GamePlayerService(
            IGamePlayerRepository gamePlayerRepository,
            IStatisticRepository statisticRepository,
            IMapper mapper)
        {
            _gamePlayerRepository = gamePlayerRepository;
            _statisticRepository = statisticRepository;
            _mapper = mapper;
        }

        public async Task<GamePlayerDTO> GetGamePlayerByIdAsync(int id)
        {
            var gamePlayer = await _gamePlayerRepository.GetGamePlayerWithStatsAsync(id);
            return _mapper.Map<GamePlayerDTO>(gamePlayer);
        }

        public async Task<IEnumerable<GamePlayerDTO>> GetGamePlayersByGameIdAsync(int gameId)
        {
            var gamePlayers = await _gamePlayerRepository.GetGamePlayersForGameAsync(gameId);
            return _mapper.Map<IEnumerable<GamePlayerDTO>>(gamePlayers);
        }
    }
}
