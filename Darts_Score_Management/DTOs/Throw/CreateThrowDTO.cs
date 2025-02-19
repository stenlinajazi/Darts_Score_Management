using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.Throw
{
    public class CreateThrowDTO
    {
        [Required]
        [Range(1, 3)]
        public int ThrowNumber { get; set; }

        [Required]
        [Range(0, 25)]
        public int Segment { get; set; }

        [Required]
        [Range(1, 3)]
        public int Multiplier { get; set; }
    }
}
