namespace Darts_Score_Management.DTOs.Statistic
{
    public class Last10StatsDTO
    {
        public StatSummary PPD { get; set; }
        public StatSummary First9PPD { get; set; }
        public StatSummary CheckoutPercentage { get; set; }
        public StatSummary WinPercentage { get; set; }
    }
}
