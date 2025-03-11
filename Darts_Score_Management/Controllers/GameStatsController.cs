    using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStatsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public GameStatsController(IStatisticService statisticService)
        {
            _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
        }

        [HttpGet("{gameId}/stats/leg/{legId}/{gamePlayerId}")]
        public async Task<IActionResult> GetLegStats(int gameId, int legId, int gamePlayerId)
        {
            try
            {
                var stats = await _statisticService.GetLegStatsAsync(legId, gamePlayerId);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving leg stats.", error = ex.Message });
            }
        }


        [HttpGet("{gameId}/stats/set/{setId}/{gamePlayerId}")]
        public async Task<IActionResult> GetSetStats(int gameId, int setId, int gamePlayerId)
        {
            try
            {
                var stats = await _statisticService.GetSetStatsAsync(setId, gamePlayerId);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving set stats.", error = ex.Message });
            }
        }


        [HttpGet("{gameId}/stats/{gamePlayerId}")]
        public async Task<IActionResult> GetGameStats(int gameId, int gamePlayerId)
        {
            try
            {
                var stats = await _statisticService.GetGameStatsAsync(gameId, gamePlayerId);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving game stats.", error = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetGameHistory()
        {
            try
            {
                var history = await _statisticService.GetGameHistoryAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving game history.", error = ex.Message });
            }
        }
    }
}
