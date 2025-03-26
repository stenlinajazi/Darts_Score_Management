using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegsController : ControllerBase
    {
        private readonly ILegService _legService;

        public LegsController(ILegService legService)
        {
            _legService = legService;
        }

        [HttpGet("set/{setId}")]  
        public async Task<ActionResult<IEnumerable<LegDTO>>> GetLegsBySet(int setId)
        {
            try
            {
                var legs = await _legService.GetLegsBySetIdAsync(setId);
                return Ok(legs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
        }


        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Leg>> GetLeg(int id)
        {
            try
            {
                var leg = await _legService.GetLegByIdAsync(id);
                return Ok(leg);
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

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LegDTO>> CreateLeg(CreateLegDTO createLegDto)
        {
            try
            {
                var leg = await _legService.CreateLegAsync(createLegDto);
                return CreatedAtAction(nameof(GetLeg), new { id = leg.Id }, leg);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
        }


        [HttpPatch("{id}/end")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LegDTO>> EndLeg(int id, [FromBody] int winnerId)
        {
            try
            {
                var leg = await _legService.EndLegAsync(id, winnerId);
                return Ok(leg);
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
