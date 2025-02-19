namespace Darts_Score_Management.Data.Models
{
    public class Leg
    {
        public int Id { get; set; }
        public int SetId { get; set; }
        public int LegNumber { get; set; }
        public int? WinnerPlayerId { get; set; }

        // Navigation properties
        public Set Set { get; set; }
        public Player Winner { get; set; }
        public List<Turn> Turns { get; set; }
    }
}
