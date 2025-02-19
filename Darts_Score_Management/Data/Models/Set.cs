namespace Darts_Score_Management.Data.Models
{
    public class Set
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int SetNumber { get; set; }
        public int? WinnerPlayerId { get; set; }

        // Navigation properties
        public Game Game { get; set; }
        public Player Winner { get; set; }
        public List<Leg> Legs { get; set; }
    }
}
