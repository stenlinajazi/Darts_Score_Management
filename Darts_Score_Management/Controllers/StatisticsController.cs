using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        // GET: api/statistics/5
        //Single statistic record
        [HttpGet("{id}")]
        public async Task<ActionResult<StatisticDTO>> GetStatistic(int id)
        {
            var statistic = await _statisticService.GetStatisticByIdAsync(id);
            if (statistic == null)
            {
                return NotFound();
            }
            return Ok(statistic);
        }

        // GET: api/statistics/gameplayer/5
        //Single game view (specific game)
        [HttpGet("gameplayer/{gamePlayerId}")]
        public async Task<ActionResult<IEnumerable<StatisticDTO>>> GetGamePlayerStatistics(int gamePlayerId)
        {
            var statistics = await _statisticService.GetPlayerGameStatisticsAsync(gamePlayerId);
            return Ok(statistics);
        }

        // PUT: api/statistics/gameplayer/5
        [HttpPut("gameplayer/{gamePlayerId}")]
        public async Task<ActionResult<IEnumerable<StatisticDTO>>> UpdateGamePlayerStatistics(int gamePlayerId, List<StatisticDTO> stats)
        {
            var statistics = await _statisticService.UpdateStatisticsAsync(gamePlayerId, stats);
            return Ok(statistics);
        }
    }
}

