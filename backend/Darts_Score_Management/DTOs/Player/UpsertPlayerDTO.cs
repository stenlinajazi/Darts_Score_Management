using Darts_Score_Management.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.DTOs.Player
{
    public class UpsertPlayerDTO
    {
        [Required, MaxLength(100)]
        [PlayerNameValidation]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        [UsernameValidation]
        public string Username { get; set; }

        [MaxLength(255)]
        public string ProfileImageUrl { get; set; }
    }
}
