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
            var legs = await _legService.GetLegsBySetIdAsync(setId);
            return Ok(legs);
        }


        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Leg>> GetLeg(int id)
        {
            var leg = await _legService.GetLegByIdAsync(id);
            if (leg == null)
              return NotFound();
                    
            return Ok(leg);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
