namespace Darts_Score_Management.Data.Models
{
    public class BustAnalysisResult
    {
        public bool HasBust { get; set; }
        public int BustIndex { get; set; } = -1;
        public string BustMessage { get; set; }
    }
}
