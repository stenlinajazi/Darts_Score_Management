using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Throw;

namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IGameRulesEngine
    {
        //bool IsCheckoutValid(int remainingScore, List<Throw> throws);
        //bool IsGameOver(Game game);
        //bool IsSetOver(Set set, GameSettings settings);
        //bool IsLegOver(Leg leg, int startingScore);
        //int CalculateRemainingScore(int startingScore, List<Throw> throws);
        //bool IsValidThrow(Throw throw_);

        Task<ValidationResult> ValidateThrow(CreateThrowDTO throwDto, int turnId);
        Task<GameStateDTO> ProcessTurn(int turnId, List<CreateThrowDTO> throws);

    }
}
