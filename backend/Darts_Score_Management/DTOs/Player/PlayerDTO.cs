using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.Player
{
    public class PlayerDTO : UpsertPlayerDTO
    {
        [Required]
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
