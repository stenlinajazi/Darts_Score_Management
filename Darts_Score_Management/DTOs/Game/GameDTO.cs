using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.Enums;

namespace Darts_Score_Management.DTOs.Game
{
    public class GameDTO
    {
        public int Id { get; set; }
        public GameType Type { get; set; }
        public int StartingScore { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsComplete { get; set; }
        public GameSettingsDTO Settings { get; set; }
        public List<GamePlayerDTO> Players { get; set; }
        public List<SetDTO> Sets { get; set; }
    }
}
