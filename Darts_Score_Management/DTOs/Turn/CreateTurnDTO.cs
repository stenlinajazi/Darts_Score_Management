using Darts_Score_Management.DTOs.Throw;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.Turn
{
    public class CreateTurnDTO
    {
        [Required]
        public int LegId { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        public int TurnNumber { get; set; }

        [Required]
        public int StartingScore { get; set; }

        //[Required]
        //public List<CreateThrowDTO> Throws { get; set; }
    }
}
