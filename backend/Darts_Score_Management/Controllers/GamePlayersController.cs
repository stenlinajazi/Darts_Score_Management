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
            try
            {
                var gamePlayers = await _gamePlayerService.GetGamePlayersByGameIdAsync(gameId);
                return Ok(gamePlayers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GamePlayerDTO>> GetGamePlayer(int id)
        {
            try
            {
                var gamePlayer = await _gamePlayerService.GetGamePlayerByIdAsync(id);
                return Ok(gamePlayer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
