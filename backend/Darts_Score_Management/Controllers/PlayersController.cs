using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDTO>>> GetPlayers()
        {
            try
            {
                var players = await _playerService.GetAllPlayersAsync();
                return Ok(players);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PlayerDTO>>> GetActivePlayers()
        {
            var players = await _playerService.GetActivePlayersAsync();
            return Ok(players);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDTO>> GetPlayer(int id)
        {
            try
            {
                var player = await _playerService.GetPlayerByIdAsync(id);
                return Ok(player);
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

        //[HttpGet("{id}/stats")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<ActionResult<PlayerStatsDTO>> GetPlayerWithStats(int id)
        //{
        //    try
        //    {
        //        var player = await _playerService.GetPlayerWithStatsAsync(id);
        //        return Ok(player);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}


        [HttpPost]
        public async Task<ActionResult<PlayerDTO>> CreatePlayer(UpsertPlayerDTO createPlayerDto)
        {
            try
            {
                var player = await _playerService.CreatePlayerAsync(createPlayerDto);
                return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Validation Error", Detail = ex.Message });
            }
        }

      
        [HttpPut("{id}")]
        public async Task<ActionResult<PlayerDTO>> UpdatePlayer(int id, UpsertPlayerDTO upsertPlayerDto)
        {
            try
            {
                var player = await _playerService.UpdatePlayerAsync(id, upsertPlayerDto);
                return Ok(player);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Validation Error", Detail = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            try
            {
                await _playerService.DeletePlayerAsync(id);
                return NoContent();
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
