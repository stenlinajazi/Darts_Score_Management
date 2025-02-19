using Darts_Score_Management.Data.Models.Bases;
using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.Data.Models
{
    public class Player : BaseModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(255)]
        public string ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }
       

        // Navigation properties
        public List<GamePlayer> GamePlayers { get; set; }
        public List<Turn> Turns { get; set; }
        public List<Leg> WonLegs { get; set; }
        public List<Set> WonSets { get; set; }
    }
}
