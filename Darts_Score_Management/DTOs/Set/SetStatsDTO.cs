namespace Darts_Score_Management.DTOs.Set
{
    public class SetStatsDTO
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; }
        public int SetId { get; set; }
        public int LegsWin { get; set; }
        public decimal PPD { get; set; }
        public decimal First9PPD { get; set; }
        public string CheckoutPercentage { get; set; }
        public int Count60Plus { get; set; }
        public int Count100Plus { get; set; }
        public int Count140Plus { get; set; }
        public int Count180s { get; set; }
        public int HighestCheckout { get; set; }
    }
}
