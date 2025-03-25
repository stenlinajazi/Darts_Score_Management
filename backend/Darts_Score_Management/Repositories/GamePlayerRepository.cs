using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class GamePlayerRepository : BaseRepository<GamePlayer>, IGamePlayerRepository
    {
        public GamePlayerRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<GamePlayerDTO>> GetGamePlayersForGameAsync(int gameId)
        {
            return await _context.GamePlayers
                .Where(gp => gp.GameId == gameId)
                .OrderBy(gp => gp.TurnOrder)
                .Select(gp => new GamePlayerDTO 
                { 
                    Id = gp.Id,
                    GameId = gp.GameId,
                    Player = new PlayerDTO
                    {
                        Id = gp.Player.Id,
                        Name = gp.Player.Name,
                        Username = gp.Player.Username,
                        ProfileImageUrl = gp.Player.ProfileImageUrl,
                        IsActive=gp.Player.IsActive },
                    TurnOrder = gp.TurnOrder,
                    IsWinner = gp.IsWinner,
                    FinalRanking = gp.FinalRanking
                }) 
                .ToListAsync();
        }

        public async Task<GamePlayerDTO> GetGamePlayerAsync(int gamePlayerId)
        {
            return await _context.GamePlayers
                .Where(gp => gp.Id == gamePlayerId)
                 .Select(gp => new GamePlayerDTO
                 {
                     Id = gp.Id,
                     GameId = gp.GameId,
                     Player = new PlayerDTO
                     {
                         Id = gp.Player.Id,
                         Name = gp.Player.Name,
                         Username = gp.Player.Username,
                         ProfileImageUrl = gp.Player.ProfileImageUrl,
                         IsActive = gp.Player.IsActive
                     },
                     TurnOrder = gp.TurnOrder,
                     IsWinner = gp.IsWinner,
                     FinalRanking = gp.FinalRanking
                 })
                 .FirstOrDefaultAsync();
        }
    }
}

