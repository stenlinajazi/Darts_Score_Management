namespace Darts_Score_Management.DTOs.Game.Core
{
    public class GameSettingsDTO
    {

        public bool MustFinishOnDouble { get; set; }
        public int SetsToWin { get; set; }
        public int LegsPerSet { get; set; }
    }
}
