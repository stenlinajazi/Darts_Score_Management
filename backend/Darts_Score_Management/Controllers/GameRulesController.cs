using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game.State;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Darts_Score_Management.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameRulesController : ControllerBase
    {
        private readonly IGameRulesEngine _gameRulesEngine;
        private readonly IGameService _gameService;

        public GameRulesController(IGameRulesEngine gameRulesEngine, IGameService gameService)
        {
            _gameRulesEngine = gameRulesEngine;
            _gameService = gameService;
        }

        [HttpPost("throws")]
        public async Task<ActionResult<GameStateDTO>> ProcessTurn([FromBody] List<CreateThrowDTO> throws, [FromQuery] int? gameId = null)
        {
            if (throws == null || throws.Count > 3)
                throw new GameRuleViolationException("A turn must contain 0 to 3 throws", "ThrowCount");

            int legId;
            if (gameId.HasValue)
            {
                legId = await _gameService.GetActiveLegIdByGameIdAsync(gameId.Value);
            }
            else
            {
                legId = await _gameService.GetActiveLegIdAsync();
            }

            var gameState = await _gameRulesEngine.ProcessTurnForLeg(legId, throws);
            return Ok(gameState);
        }
    }
}
