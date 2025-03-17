using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class GameValidationService : IGameValidationService
    {
        private readonly ITurnService _turnService;
        private readonly IGameService _gameService;
        private const int MaxThrowsPerTurn = 3;

        public GameValidationService(ITurnService turnService, IGameService gameService)
        {
            _turnService = turnService;
            _gameService = gameService;
        }

        public async Task<bool> ValidateTurnOrder(int turnId, int playerId)
        {
            var turn = await _turnService.GetTurnByIdAsync(turnId);
            return turn?.PlayerId == playerId;
        }

        //public async Task<bool> ValidateMaximumThrows(int turnId)
        //{
        //    var turn = await _turnService.GetTurnByIdAsync(turnId);
        //    return turn?.Throws.Count < MaxThrowsPerTurn;
        //}

        //public async Task<bool> ValidateGameInProgress(int gameId)
        //{
        //    var game = await _gameService.GetGameByIdAsync(gameId);
        //    return game != null && !game.IsComplete;
        //}
    }
}
