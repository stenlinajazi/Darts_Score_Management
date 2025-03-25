using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Statistic;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.GamePlayer
{
    public class GamePlayerDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required(ErrorMessage = "Player is required.")]
        public PlayerDTO Player { get; set; }
        [Required]
        public int TurnOrder { get; set; }
        public bool IsWinner { get; set; }
        [Required]
        public int FinalRanking { get; set; }
    }
}
