using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamePlayersController : ControllerBase
    {
        private readonly IGamePlayerService _gamePlayerService;

        public GamePlayersController(IGamePlayerService gamePlayerService)
        {
            _gamePlayerService = gamePlayerService;
        }

        // GET: api/gameplayers/game/5
        [HttpGet("game/{gameId}")]
        public async Task<ActionResult<IEnumerable<GamePlayerDTO>>> GetGamePlayers(int gameId)
        {
            var gamePlayers = await _gamePlayerService.GetGamePlayersByGameIdAsync(gameId);
            return Ok(gamePlayers);
        }

        // GET: api/gameplayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GamePlayerDTO>> GetGamePlayer(int id)
        {
            var gamePlayer = await _gamePlayerService.GetGamePlayerByIdAsync(id);
            if (gamePlayer == null)
            {
                return NotFound();
            }
            return Ok(gamePlayer);
        }
    }
}
