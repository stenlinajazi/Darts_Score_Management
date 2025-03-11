using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Game;
using System.Text.Json.Serialization;

namespace Darts_Score_Management.DTOs.Set
{
    public class SetDTO
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        //[JsonIgnore]
        //public GameDTO Game { get; set; }
        public int SetNumber { get; set; }
        public int? WinnerPlayerId { get; set; }
        public PlayerDTO Winner { get; set; }
        public List<LegDTO> Legs { get; set; }
    }
}
