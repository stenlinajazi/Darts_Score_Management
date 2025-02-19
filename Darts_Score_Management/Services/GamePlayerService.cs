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

        //public async Task<GamePlayerDTO> UpdateGamePlayerStatsAsync(int id, List<StatisticDTO> stats)
        //{
        //    var gamePlayer = await _gamePlayerRepository.GetByIdAsync(id);
        //    if (gamePlayer == null)
        //        throw new KeyNotFoundException($"GamePlayer with id {id} not found");

        //    var statistics = _mapper.Map<List<Statistic>>(stats);
        //    await _statisticRepository.UpdateStatisticsAsync(id, statistics);

        //    return await GetGamePlayerByIdAsync(id);
        //}

        //public async Task<GamePlayerDTO> SetGamePlayerWinnerAsync(int gameId, int playerId)
        //{
        //    var gamePlayers = await _gamePlayerRepository.GetGamePlayersForGameAsync(gameId);
        //    var winner = gamePlayers.FirstOrDefault(gp => gp.PlayerId == playerId);

        //    if (winner == null)
        //        throw new KeyNotFoundException($"Player with id {playerId} not found in game {gameId}");

        //    winner.IsWinner = true;
        //    await _gamePlayerRepository.UpdateAsync(winner);

        //    return _mapper.Map<GamePlayerDTO>(winner);
        //}
    }
}
