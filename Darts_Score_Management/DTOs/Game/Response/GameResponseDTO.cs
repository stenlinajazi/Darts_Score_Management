namespace Darts_Score_Management.DTOs.Game.Response
{
    public class GameResponseDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int StartingScore { get; set; }
        public int SetsToWin { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsComplete { get; set; }
        public int? WinnerId { get; set; }
    }
}
