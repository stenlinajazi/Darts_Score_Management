using Darts_Score_Management.Enums;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.Game.Core
{
    public class CreateGameDTO
    {
        [Required]
        public GameType Type { get; set; }

        [Required]
        public int StartingScore { get; set; }

        [Required]
        public GameSettingsDTO Settings { get; set; }

        [Required]
        [MinLength(2)]
        public List<int> PlayerIds { get; set; }
    }
}
