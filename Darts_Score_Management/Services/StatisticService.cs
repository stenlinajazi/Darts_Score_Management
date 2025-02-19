using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _statisticRepository;
        private readonly IMapper _mapper;

        public StatisticService(IStatisticRepository statisticRepository, IMapper mapper)
        {
            _statisticRepository = statisticRepository;
            _mapper = mapper;
        }

        public async Task<StatisticDTO> GetStatisticByIdAsync(int id)
        {
            var statistic = await _statisticRepository.GetByIdAsync(id);
            return _mapper.Map<StatisticDTO>(statistic);
        }

        public async Task<IEnumerable<StatisticDTO>> GetPlayerGameStatisticsAsync(int gamePlayerId)
        {
            var statistics = await _statisticRepository.GetPlayerGameStatisticsAsync(gamePlayerId);
            return _mapper.Map<IEnumerable<StatisticDTO>>(statistics);
        }

        public async Task<IEnumerable<StatisticDTO>> UpdateStatisticsAsync(int gamePlayerId, List<StatisticDTO> stats)
        {
            var statistics = _mapper.Map<List<Statistic>>(stats);
            await _statisticRepository.UpdateStatisticsAsync(gamePlayerId, statistics);
            return await GetPlayerGameStatisticsAsync(gamePlayerId);
        }
    }
}
