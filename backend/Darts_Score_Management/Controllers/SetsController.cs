using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetsController : ControllerBase
    {
        private readonly ISetService _setService;

        public SetsController(ISetService setService)
        {
            _setService = setService;
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<SetDTO>> GetSet(int id)
        {
            var set = await _setService.GetSetByIdAsync(id);
            if (set == null)
                return NotFound();

            return Ok(set);
        }

       
        [HttpGet("game/{gameId}")]
        public async Task<ActionResult<IEnumerable<SetDTO>>> GetSetsByGame(int gameId)
        {
            var sets = await _setService.GetSetsByGameIdAsync(gameId);
            return Ok(sets);
        }

       
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<SetDTO>> CreateSet(CreateSetDTO createSetDto)
        {
            try
            {
                var set = await _setService.CreateSetAsync(createSetDto);
                return CreatedAtAction(nameof(GetSet), new { id = set.Id }, set);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPatch("{id}/end")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<SetDTO>> EndSet(int id, [FromBody] int winnerId)
        {
            try
            {
                var set = await _setService.EndSetAsync(id, winnerId);
                return Ok(set);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
