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
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
