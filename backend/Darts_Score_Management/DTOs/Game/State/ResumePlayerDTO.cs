namespace Darts_Score_Management.DTOs.Game.State
{
    public class ResumePlayerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartingScore { get; set; }
        public int PointsThisTurn { get; set; }
        public int RemainingScore { get; set; }
    }
}