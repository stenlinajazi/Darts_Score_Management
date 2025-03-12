using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Set;

namespace Darts_Score_Management.DTOs.Game
{
    public class PlayerMatchStatsDTO
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public GameStatsDTO GameStats { get; set; }
        public List<SetStatsDTO> SetStats { get; set; } = new List<SetStatsDTO>();
        public List<LegStatsDTO> LegStats { get; set; } = new List<LegStatsDTO>();
    }
}
