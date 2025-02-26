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

        [HttpPost("{legId}/throws")]
        public async Task<ActionResult<GameStateDTO>> ProcessTurn(int legId, [FromBody] List<CreateThrowDTO> throws)
        {
            try
            {
                if (throws == null || throws.Count != 3)//chack for 3 excact throws is done in the service also(maybe redundant here)
                {
                    return BadRequest(new { message = "A turn must contain exactly 3 throws" });
                }


                var gameState = await _gameRulesEngine.ProcessTurnForLeg(legId, throws);
                return Ok(gameState);
            }
            catch (GameRuleViolationException ex)
            {
                return BadRequest(new { message = ex.Message, rule = ex.RuleViolated });
            }
            catch (StatisticsUpdateException ex)
            {
     
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
