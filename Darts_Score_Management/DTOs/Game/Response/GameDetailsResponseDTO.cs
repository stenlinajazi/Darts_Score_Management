using Darts_Score_Management.DTOs.Game.Players;

namespace Darts_Score_Management.DTOs.Game.Response
{
    public class GameDetailsResponseDTO : GameResponseDTO
    {
        public List<PlayerMatchStatsDTO> Players { get; set; } = new List<PlayerMatchStatsDTO>();
    }
}
