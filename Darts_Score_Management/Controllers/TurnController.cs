using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnController : ControllerBase
    {
        private readonly ITurnService _turnService;

        public TurnController(ITurnService turnService)
        {
            _turnService = turnService;
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<TurnDTO>> GetTurn(int id)
        {
            try
            {
                var turn = await _turnService.GetTurnByIdAsync(id);
                return Ok(turn);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

      
        [HttpPost]
        public async Task<ActionResult<TurnDTO>> CreateTurn(CreateTurnDTO createTurnDto)
        {
            try
            {
                var turn = await _turnService.CreateTurnAsync(createTurnDto);
                return CreatedAtAction(nameof(GetTurn), new { id = turn.Id }, turn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

       
        [HttpPost("{turnId}/throws")]
        public async Task<ActionResult<TurnDTO>> AddThrowToTurn(int turnId, CreateThrowDTO throwDto)
        {
            try
            {
                var updatedTurn = await _turnService.AddThrowToTurnAsync(turnId, throwDto);
                return Ok(updatedTurn);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [HttpPost("{turnId}/throws/batch")]
        public async Task<ActionResult<TurnDTO>> AddMultipleThrowsToTurn(int turnId, List<CreateThrowDTO> throws)
        {
            try
            {
                TurnDTO updatedTurn = new TurnDTO();
                foreach (var throwDto in throws)
                {
                    updatedTurn = await _turnService.AddThrowToTurnAsync(turnId, throwDto);
                }
                return Ok(updatedTurn);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
