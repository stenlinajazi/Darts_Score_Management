using Darts_Score_Management.Data.Models;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IGameRulesEngine
    {
        bool IsValidTurn(Turn turn, Game game);
        bool IsValidCheckout(Turn turn, Game game);
        int CalculateTurnScore(Turn turn);
        bool HasPlayerWonLeg(Turn turn, Game game);
        bool HasPlayerWonSet(int playerId, Set set, Game game);
        bool HasPlayerWonGame(int playerId, Game game);
    }
}
