using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.Enums;

namespace Darts_Score_Management.DTOs.Game.Core
{
    public class GameDTO : BaseGameDTO
    {
        public GameSettingsDTO Settings { get; set; }
        public List<GamePlayerDTO> Players { get; set; }
        public List<SetDTO> Sets { get; set; }
    }
}
