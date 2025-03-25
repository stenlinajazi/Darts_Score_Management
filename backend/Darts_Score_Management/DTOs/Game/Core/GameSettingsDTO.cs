using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.Game.Core
{
    public class GameSettingsDTO
    {

        public bool MustFinishOnDouble { get; set; }
        [Range(1, 3, ErrorMessage = "SetsToWin must be between 1 and 3")]
        public int SetsToWin { get; set; }
        [Range(1, 3, ErrorMessage = "LegsPerSet must be between 1 and 3")]
        public int LegsPerSet { get; set; }
    }
}
