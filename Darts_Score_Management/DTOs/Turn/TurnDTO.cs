using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Throw;

namespace Darts_Score_Management.DTOs.Turn
{
    public class TurnDTO
    {
        public int Id { get; set; }
        public int LegId { get; set; }
        public LegDTO Leg { get; set; }
        public int PlayerId { get; set; }
        public int TurnNumber { get; set; }
        public int StartingScore { get; set; }
        public int EndingScore { get; set; }
        public int TotalPoints { get; set; }
        public List<ThrowDTO> Throws { get; set; }
    }
}
