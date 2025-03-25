using Darts_Score_Management.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("gameplayers/{gameId}")]
        public IActionResult GetGamePlayers(int gameId)
        {
            var query = _context.GamePlayers
                .Where(gp => gp.GameId == gameId)
                .Include(gp => gp.Player)
                .OrderBy(gp => gp.TurnOrder)
                .Select(gp => new
                {
                    GamePlayerId = gp.Id,
                    PlayerName = gp.Player.Name,
                    TurnOrder = gp.TurnOrder
                })
                .ToList();

            return Ok(query);
        }
    }
}
