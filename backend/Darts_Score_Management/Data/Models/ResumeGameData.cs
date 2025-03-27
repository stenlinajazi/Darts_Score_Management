namespace Darts_Score_Management.Data.Models
{
    public class ResumeGameData
    {
        public Game Game { get; set; }
        public Leg ActiveLeg { get; set; }
        public Turn LastTurn { get; set; }
    }
}
