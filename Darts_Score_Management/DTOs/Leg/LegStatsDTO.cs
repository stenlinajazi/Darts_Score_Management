namespace Darts_Score_Management.DTOs.Leg
{
    public class LegStatsDTO
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; }
        public int LegId { get; set; }
        public decimal PPD { get; set; }
        public decimal First9PPD { get; set; }
        public int TotalThrows { get; set; }
        public string CheckoutPercentage { get; set; }
        public int Count60Plus { get; set; }
        public int Count100Plus { get; set; }
        public int Count140Plus { get; set; }
        public int Count180s { get; set; }
    }
}
