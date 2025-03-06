namespace Darts_Score_Management.Data.Models
{
    public class Turn
    {
        public int Id { get; set; }
        public int LegId { get; set; }
        public int PlayerId { get; set; }
        public int TurnNumber { get; set; }
        public int StartingScore { get; set; }
        public int EndingScore { get; set; }
        public int TotalPoints { get; set; }
        public bool IsCheckoutAttempt { get; set; }
        public bool IsCheckoutSuccessful { get; set; }

        // Navigation properties
        public Leg Leg { get; set; }
        public Player Player { get; set; }
        public List<Throw> Throws { get; set; }
    }
}
