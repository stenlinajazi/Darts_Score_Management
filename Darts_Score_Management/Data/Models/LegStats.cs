using Darts_Score_Management.Data.Models.Bases;

namespace Darts_Score_Management.Data.Models
{
    public class LegStats : BaseModel
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; } // FK to GamePlayers
        public int LegId { get; set; } // FK to Legs
        public decimal PPD { get; set; } // Points Per Dart
        public decimal First9PPD { get; set; } // First 9 Darts PPD
        public int TotalThrows { get; set; }
        public string CheckoutPercentage { get; set; } // e.g., "100% (1/1)" or "-"
        public int Count60Plus { get; set; }
        public int Count100Plus { get; set; }
        public int Count140Plus { get; set; }
        public int Count180s { get; set; }

        // Navigation properties
        public GamePlayer GamePlayer { get; set; }
        public Leg Leg { get; set; }
    }
}
