namespace Darts_Score_Management.DTOs.Game
{
    public class GameStatsDTO
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; }
        public int GameId { get; set; }
        public int SetsWin { get; set; }
        public int LegsWin { get; set; }
        public decimal PPD { get; set; }
        public decimal First9PPD { get; set; }
        public string CheckoutPercentage { get; set; }
        public int Count60Plus { get; set; }
        public int Count100Plus { get; set; }
        public int Count140Plus { get; set; }
        public int Count180s { get; set; }
    }
}
