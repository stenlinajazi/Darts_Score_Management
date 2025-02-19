using Darts_Score_Management.Data.Models.Bases;
using Darts_Score_Management.Enums;
using System.Security.AccessControl;

namespace Darts_Score_Management.Data.Models
{
    public class Game : BaseModel
    {
        public int Id { get; set; }
        public GameType Type { get; set; } // Enum: X01 (301, 501, etc.), Cricket, etc.
        public int StartingScore { get; set; } // 501, 301, etc. for X01 games
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsComplete { get; set; }
        public GameSettings Settings { get; set; } // JSON serialized or separate table

        // Navigation properties
        public List<GamePlayer> GamePlayers { get; set; }
        public List<Set> Sets { get; set; }
    }
}
