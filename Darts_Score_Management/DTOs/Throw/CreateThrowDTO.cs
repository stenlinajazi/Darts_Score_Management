using Darts_Score_Management.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.Throw
{
    public class CreateThrowDTO
    {
        //[Required]
        //[Range(1, 3)]
        //public int ThrowNumber { get; set; }

        [Required]
        // [Range(0, 25)]
        [ValidDartSegment]
        public int Segment { get; set; }

        [Required]
        //  [Range(1, 3)]
        [ValidMultiplier]
        public int Multiplier { get; set; }
    }
}
