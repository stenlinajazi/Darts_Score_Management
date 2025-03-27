using Darts_Score_Management.DTOs.Throw;

namespace Darts_Score_Management.DTOs.Game.State
{
    public class ResumeGameStateDTO
    {
        public int GameId { get; set; }
        public int StartingScore { get; set; }
        public List<ResumePlayerDTO> Players { get; set; }
        public int ActivePlayerIndex { get; set; }
        public List<CreateThrowDTO> CurrentThrows { get; set; } 
        public string Message { get; set; }

        public int SetsToWin { get; set; }
        public int LegsPerSet { get; set; }
        public int CurrentSetNumber { get; set; }
        public int CurrentLegNumber { get; set; }
        public Dictionary<int, int> SetScores { get; set; } // PlayerId -> Sets Won
    }
}
