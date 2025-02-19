/*
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class GameRulesEngine : IGameRulesEngine
    {
        private readonly IGameRepository _gameRepository;

        public GameRulesEngine(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public bool IsValidTurn(Turn turn, Game game)
        {
            // Check if game is complete
            if (game.IsComplete)
                return false;

            // Calculate score
            int turnScore = CalculateTurnScore(turn);
            int remainingScore = turn.ScoreBeforeTurn - turnScore;

            // Check for bust (score below zero)
            if (remainingScore < 0)
                return false;

            // Check if score is exactly zero (checkout)
            if (remainingScore == 0)
            {
                return IsValidCheckout(turn, game);
            }

            return true;
        }

        public bool IsValidCheckout(Turn turn, Game game)
        {
            if (!game.Settings.MustFinishOnDouble)
                return true;

            // Find the last throw that would be a checkout
            var lastThrow = turn.Throws.LastOrDefault(t => !t.IsBounceOut && t.Score > 0);
            if (lastThrow == null)
                return false;

            // Must finish on a double
            return lastThrow.Segment == DartSegment.Double || lastThrow.Segment == DartSegment.OuterBullseye;
        }

        public int CalculateTurnScore(Turn turn)
        {
            return turn.Throws.Where(t => !t.IsBounceOut).Sum(t => t.Score);
        }

        public bool HasPlayerWonLeg(Turn turn, Game game)
        {
            // If we have a valid checkout, player has won the leg
            return turn.ScoreAfterTurn == 0 && IsValidCheckout(turn, game);
        }

        public bool HasPlayerWonSet(int playerId, Set set, Game game)
        {
            // Count legs won by this player
            int legsWon = set.Legs.Count(l => l.WinnerId == playerId);

            // Winner needs legsPerSet legs
            return legsWon >= game.Settings.LegsPerSet;
        }

        public bool HasPlayerWonGame(int playerId, Game game)
        {
            // Count sets won by this player
            int setsWon = game.Sets.Count(s => s.WinnerId == playerId);

            // Winner needs setsToWin sets
            return setsWon >= game.Settings.SetsToWin;
        }
    }
}
*/