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

        // GET: api/games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetGames()
        {
            var games = await _gameService.GetAllGamesAsync();
            return Ok(games);
        }

        // GET: api/games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDTO>> GetGame(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
                return NotFound();
            return Ok(game);
        }

        // GET: api/games/player/5
        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetPlayerGames(int playerId)
        {
            var games = await _gameService.GetPlayerGamesAsync(playerId);
            return Ok(games);
        }

        // POST: api/games
        [HttpPost]
        public async Task<ActionResult<GameDTO>> CreateGame(CreateGameDTO createGameDto)
        {
            var game = await _gameService.CreateGameAsync(createGameDto);
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        // PUT: api/games/5
        [HttpPut("{id}")]
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

        // POST: api/games/5/end
        [HttpPost("{id}/end")]
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

        // DELETE: api/games/5
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
