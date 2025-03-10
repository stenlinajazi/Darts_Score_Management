using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetGames()
        {
            var games = await _gameService.GetAllGameSummariesAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameDTO>> GetGame(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<PlayerGameSummaryDTO>>> GetPlayerGames(int playerId)
        {
            var games = await _gameService.GetPlayerGamesAsync(playerId);
            return Ok(games);
        }

        [HttpPost]
        public async Task<ActionResult<GameDTO>> CreateGame(CreateGameDTO createGameDto)
        {
            var game = await _gameService.CreateGameAsync(createGameDto);
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }
     
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GameDTO>> UpdateGame(int id, GameDTO gameDto)
        {
            try
            {
                var game = await _gameService.UpdateGameAsync(id, gameDto);
                return Ok(game);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/end")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GameDTO>> EndGame(int id, [FromBody] int winnerId)
        {
            try
            {
                var game = await _gameService.EndGameAsync(id, winnerId);
                return Ok(game);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                await _gameService.DeleteGameAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
