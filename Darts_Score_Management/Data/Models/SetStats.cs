using Darts_Score_Management.Data.Models.Bases;

namespace Darts_Score_Management.Data.Models
{
    public class SetStats : BaseModel
    {
        public int Id { get; set; }
        public int GamePlayerId { get; set; } // FK to GamePlayers
        public int SetId { get; set; } // FK to Sets
        public int LegsWin { get; set; }
        public decimal PPD { get; set; }
        public decimal First9PPD { get; set; }
        public string CheckoutPercentage { get; set; } // e.g., "50.0% (2/4)"
        public int Count60Plus { get; set; }
        public int Count100Plus { get; set; }
        public int Count140Plus { get; set; }
        public int Count180s { get; set; }
        public int HighestCheckout { get; set; }

        // Navigation properties
        public GamePlayer GamePlayer { get; set; }
        public Set Set { get; set; }
    }
}
