using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Turn;
using System.Text.Json.Serialization;

namespace Darts_Score_Management.DTOs.Leg
{
    public class LegDTO
    {
        public int Id { get; set; }
        public int SetId { get; set; }
        //[JsonIgnore]
        //public SetDTO Set { get; set; }
        public int LegNumber { get; set; }
        public int? WinnerPlayerId { get; set; }
        public PlayerDTO Winner { get; set; }
        public List<TurnDTO> Turns { get; set; }
    }
}
