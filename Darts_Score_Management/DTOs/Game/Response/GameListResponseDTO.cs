using Darts_Score_Management.DTOs.Game.Players;

namespace Darts_Score_Management.DTOs.Game.Response
{
    public class GameListResponseDTO : GameResponseDTO
    {  
        public List<GamePlayerInfoDTO> Players { get; set; } = new List<GamePlayerInfoDTO>(); 
    }
}
