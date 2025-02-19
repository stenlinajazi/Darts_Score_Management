using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Repositories
{
    public class TurnRepository : BaseRepository<Turn>, ITurnRepository
    {
        public TurnRepository(AppDbContext context) : base(context) { }

        public async Task<Turn> GetTurnWithThrowsAsync(int id)
        {
            return await _context.Turns
                .Include(t => t.Throws)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Turn> AddTurnWithThrowsAsync(Turn turn)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Turns.AddAsync(turn);
                await _context.SaveChangesAsync();

                if (turn.Throws != null && turn.Throws.Any())
                {
                    foreach (var throw_ in turn.Throws)
                    {
                        throw_.TurnId = turn.Id;
                    }
                    await _context.Throws.AddRangeAsync(turn.Throws);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return turn;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
