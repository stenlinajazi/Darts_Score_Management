using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        [HttpGet("game/{gameId}")]
        public async Task<ActionResult<IEnumerable<GamePlayerDTO>>> GetGamePlayers(int gameId)
        {
            if (gameId <= 0)
            {
                ModelState.AddModelError("gameId", "Game ID must be a positive number.");
                return BadRequest(ModelState);
            }
            var gamePlayers = await _gamePlayerService.GetGamePlayersByGameIdAsync(gameId);
            return Ok(gamePlayers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GamePlayerDTO>> GetGamePlayer(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("id", "ID must be a positive number.");
                return BadRequest(ModelState);
            }
            var gamePlayer = await _gamePlayerService.GetGamePlayerByIdAsync(id);
            if (gamePlayer == null)
            {
                return NotFound();
            }
            return Ok(gamePlayer);
        }
    }
}
