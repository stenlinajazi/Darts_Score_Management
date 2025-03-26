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
            try
            {
                var set = await _setService.GetSetByIdAsync(id);
                return Ok(set);
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

       
        [HttpGet("game/{gameId}")]
        public async Task<ActionResult<IEnumerable<SetDTO>>> GetSetsByGame(int gameId)
        {
            try
            {
                var sets = await _setService.GetSetsByGameIdAsync(gameId);
                return Ok(sets);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
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
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
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
