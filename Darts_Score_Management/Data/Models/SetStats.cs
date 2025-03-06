using Darts_Score_Management.Data.Models.Bases;

namespace Darts_Score_Management.Data.Models
{
    public class SetStats : BaseModel
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; } 
        public int SetId { get; set; } 
        public int LegsWin { get; set; }
        public decimal PPD { get; set; }
        public decimal First9PPD { get; set; }
        public int CheckoutPercentage { get; set; }
        public int Count60Plus { get; set; }
        public int Count100Plus { get; set; }
        public int Count140Plus { get; set; }
        public int Count180s { get; set; }
        public int TotalThrows { get; set; }

        // Navigation properties
        public GamePlayer GamePlayer { get; set; }
        public Set Set { get; set; }
    }
}
