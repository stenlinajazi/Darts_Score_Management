namespace Darts_Score_Management.Interfaces.ServiceInterfaces
{
    public interface IGameValidationService
    {
        Task<bool> ValidateTurnOrder(int turnId, int playerId);
        //Task<bool> ValidateMaximumThrows(int turnId);
        //Task<bool> ValidateGameInProgress(int gameId);
    }
}
