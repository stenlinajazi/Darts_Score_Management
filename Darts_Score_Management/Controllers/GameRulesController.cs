using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameRulesController : ControllerBase
    {
        private readonly IGameRulesEngine _gameRulesEngine;

        public GameRulesController(IGameRulesEngine gameRulesEngine)
        {
            _gameRulesEngine = gameRulesEngine;
        }

        [HttpPost("{turnId}/validate")]
        public async Task<IActionResult> ValidateThrow(int turnId, [FromBody] CreateThrowDTO throwDto)
        {
            try
            {
                var result = await _gameRulesEngine.ValidateThrow(throwDto, turnId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{turnId}/throws")]
        public async Task<ActionResult<GameStateDTO>> ProcessTurn(int turnId, [FromBody] List<CreateThrowDTO> throws)
        {
            try
            {
                if (throws == null || !throws.Any() || throws.Count > 3)
                {
                    return BadRequest(new { message = "A turn must contain between 1 and 3 throws" });
                }

                var gameState = await _gameRulesEngine.ProcessTurn(turnId, throws);
                return Ok(gameState);
            }
            catch (GameRuleViolationException ex)
            {
                return BadRequest(new { message = ex.Message, rule = ex.RuleViolated });
            }
            catch (StatisticsUpdateException ex)
            {
                // Log the statistics update failure but don't fail the request
                return Ok(new
                {
                    message = "Turn processed successfully but statistics update failed",
                    statisticsError = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
