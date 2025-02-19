using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        // GET: api/players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDTO>>> GetPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();
            return Ok(players);
        }

        // GET: api/players/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PlayerDTO>>> GetActivePlayers()
        {
            var players = await _playerService.GetActivePlayersAsync();
            return Ok(players);
        }

        // GET: api/players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDTO>> GetPlayer(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
                return NotFound();
            return Ok(player);
        }

        [HttpGet("{id}/stats")]
        public async Task<ActionResult<PlayerDTO>> GetPlayerWithStats(int id)
        {
            try
            {
                var player = await _playerService.GetPlayerWithStatsAsync(id);
                return Ok(player);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/players
        [HttpPost]
        public async Task<ActionResult<PlayerDTO>> CreatePlayer(UpsertPlayerDTO createPlayerDto)
        {
            var player = await _playerService.CreatePlayerAsync(createPlayerDto);
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        // PUT: api/players/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PlayerDTO>> UpdatePlayer(int id, UpsertPlayerDTO upsertPlayerDto)
        {
            try
            {
                var player = await _playerService.UpdatePlayerAsync(id, upsertPlayerDto);
                return Ok(player);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            try
            {
                await _playerService.DeletePlayerAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
