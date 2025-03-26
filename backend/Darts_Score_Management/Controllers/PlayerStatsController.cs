using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerStatsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public PlayerStatsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("{playerId}/stats")]
        public async Task<IActionResult> GetPlayerStats(int playerId)
        {
            try
            {
                var stats = await _statisticService.GetPlayerStatsAsync(playerId);
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
