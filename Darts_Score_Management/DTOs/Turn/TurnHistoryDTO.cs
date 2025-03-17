using Darts_Score_Management.DTOs.Throw;

namespace Darts_Score_Management.DTOs.Turn
{
    public class TurnHistoryDTO
    {
        public int TurnId { get; set; }
        public int EndingScore { get; set; }
        public List<CreateThrowDTO> Throws { get; set; } = new List<CreateThrowDTO>();
    }
}
