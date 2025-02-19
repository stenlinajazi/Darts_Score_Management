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
                return NotFound();
            return Ok(gamePlayer);
        }

        // PUT: api/gameplayers/5/statistics
        //[HttpPut("{id}/statistics")]
        //public async Task<ActionResult<GamePlayerDTO>> UpdateGamePlayerStats(int id, List<StatisticDTO> stats)
        //{
        //    try
        //    {
        //        var gamePlayer = await _gamePlayerService.UpdateGamePlayerStatsAsync(id, stats);
        //        return Ok(gamePlayer);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        // POST: api/gameplayers/game/5/winner/3
        //[HttpPost("game/{gameId}/winner/{playerId}")]
        //public async Task<ActionResult<GamePlayerDTO>> SetGameWinner(int gameId, int playerId)
        //{
        //    try
        //    {
        //        var gamePlayer = await _gamePlayerService.SetGamePlayerWinnerAsync(gameId, playerId);
        //        return Ok(gamePlayer);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}
    }
}
