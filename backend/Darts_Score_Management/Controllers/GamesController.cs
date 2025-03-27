using Darts_Score_Management.DTOs.Game.Core;
using Darts_Score_Management.DTOs.Game.Response;
using Darts_Score_Management.DTOs.Game.State;
using Darts_Score_Management.DTOs.Game.Statistics;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public async Task<ActionResult<IEnumerable<GameListResponseDTO>>> GetGames()
        {
            var games = await _gameService.GetAllSummariesAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameDetailsResponseDTO>> GetGame(int id)
        {
            try
            {
                var game = await _gameService.GetGameWithDetailsAndHistoryAsync(id);
                return Ok(game);
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


        [HttpGet("{gameId}/state")]
        public async Task<ActionResult<ResumeGameStateDTO>> GetResumeGameState(int gameId)
        {
            try
            {
                var gameState = await _gameService.GetResumeGameStateAsync(gameId);
                return Ok(gameState);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Argument", Detail = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid Operation", Detail = ex.Message });
            }
        }


        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<PlayerGameSummaryDTO>>> GetPlayerGames(int playerId)
        {
            try
            {
                var games = await _gameService.GetPlayerGamesAsync(playerId);
                return Ok(games);
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
        public async Task<ActionResult<GameDTO>> CreateGame(CreateGameDTO createGameDto)
        {
            try
            {
                var game = await _gameService.CreateGameAsync(createGameDto);
                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Argument",
                    Detail = ex.Message
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Error",
                    Detail = ex.Message
                });
            }
        }
     
        //[HttpPut("{id}")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<ActionResult<GameDTO>> UpdateGame(int id, GameDTO gameDto)
        //{
        //    try
        //    {
        //        var game = await _gameService.UpdateGameAsync(id, gameDto);
        //        return Ok(game);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        //[HttpPost("{id}/end")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<ActionResult<GameDTO>> EndGame(int id, [FromBody] int winnerId)
        //{
        //    try
        //    {
        //        var game = await _gameService.EndGameAsync(id, winnerId);
        //        return Ok(game);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                await _gameService.DeleteGameAsync(id);
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
