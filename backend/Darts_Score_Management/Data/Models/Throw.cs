namespace Darts_Score_Management.Data.Models
{
    public class Throw
    {
        public int Id { get; set; }
        public int TurnId { get; set; }
        public int ThrowNumber { get; set; } // 1, 2, or 3 within a turn
        public int Segment { get; set; } // 1-20, 25 for bull
        public int Multiplier { get; set; } // 1, 2, or 3 (S, D, T)
        public bool IsBusted { get; set; } // If this throw caused a bust

        // Navigation properties
        public Turn Turn { get; set; }

        // Calculated property
        public int Points => Segment * Multiplier;
    }
}
