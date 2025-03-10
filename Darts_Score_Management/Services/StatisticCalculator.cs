using Darts_Score_Management.Data.Models;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Darts_Score_Management.Services
{
    public static class StatisticCalculator
    {
        // Calculates Points Per Dart (PPD) for different statistical collections
        public static decimal CalculatePPD(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            var totalPoints = turns.Sum(t => t.TotalPoints);
            var totalDarts = turns.Sum(t => t.Throws.Count);
            return CalculatePPDFromPointsAndThrows(totalPoints, totalDarts);
        } 
        
        // Calculates Points Per Dart for Leg Statistics
        public static decimal CalculatePPDAggregate(IEnumerable<LegStats> legStats)
        {
            if (IsNullOrEmpty(legStats)) return 0;
            var totalPoints = legStats.Sum(ls => ls.PPD * ls.TotalThrows);
            var totalThrows = legStats.Sum(ls => ls.TotalThrows);
            return CalculatePPDFromPointsAndThrows(totalPoints, totalThrows);
        }
        
        // Calculates Points Per Dart for Set Statistics
        public static decimal CalculatePPDAggregate(IEnumerable<SetStats> setStats)
        {
            if (IsNullOrEmpty(setStats)) return 0;
            var totalPoints = setStats.Sum(ss => ss.PPD * ss.TotalThrows);
            var totalThrows = setStats.Sum(ss => ss.TotalThrows);
            return CalculatePPDFromPointsAndThrows(totalPoints, totalThrows);
        }

        // Calculates First 9 Darts PPD for Turns
        // Important: When a turn is busted, we still count the darts in the denominator,
        // but we don't add the points to the numerator
        public static decimal CalculateFirst9PPD(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            var orderedTurns = turns.OrderBy(t => t.TurnNumber).ToList();

            decimal totalPoints = 0;
            int dartCount = 0;

            foreach (var turn in orderedTurns)
            {
                bool turnIsBusted = turn.Throws.Any(th => th.IsBusted);

                // For each turn, count all darts toward the 9 total
                // but only add points if the turn isn't busted
                int turnsThrowCount = turn.Throws.Count;

                // If adding all darts from this turn would exceed 9, limit it
                int dartsToCount = Math.Min(turnsThrowCount, 9 - dartCount);

                if (dartsToCount <= 0) break;

                // Only add points if the turn isn't busted
                if (!turnIsBusted)
                {
                    // Sum the points from this turn's throws (up to our limit) 
                    var throwPoints = turn.Throws
                        .Take(dartsToCount)
                        .Sum(th => th.Multiplier * th.Segment);

                    totalPoints += throwPoints;
                }

                // Always increment the dart count
                dartCount += dartsToCount;

                if (dartCount >= 9) break;
            }

            // Always divide by the actual number of darts counted, up to 9
            return CalculatePPDFromPointsAndThrows(totalPoints, dartCount);
        }

        public static decimal CalculateFirst9PPDAggregate(IEnumerable<LegStats> legStats)
        {
            if (IsNullOrEmpty(legStats)) return 0;

            decimal totalWeightedPoints = 0;
            int totalDarts = 0;

            // For each leg, consider its first 9 darts
            foreach (var legStat in legStats)
            {
                // Each leg contributes its First9PPD * the number of darts used (up to 9)
                // For First9PPD, we assume this is always calculated based on at most 9 darts
                int dartsInThisLeg = legStat.TotalThrows >= 9 ? 9 : legStat.TotalThrows;

                // Calculate weighted points contribution
                decimal pointsFromThisLeg = legStat.First9PPD * dartsInThisLeg;

                totalWeightedPoints += pointsFromThisLeg;
                totalDarts += dartsInThisLeg;
            }

            return totalDarts > 0 ? Math.Round(totalWeightedPoints / totalDarts, 2) : 0;
        }

        // Calculates First 9 Darts PPD for Set Statistics
        // Follows the same logic as for leg statistics
        public static decimal CalculateFirst9PPDAggregate(IEnumerable<SetStats> setStats)
        {
            if (IsNullOrEmpty(setStats)) return 0;

            decimal totalWeightedPoints = 0;
            int totalDarts = 0;

            // For each set, consider its First9PPD contribution
            foreach (var setStat in setStats)
            {
                // Each set contributes its First9PPD * the number of darts used (up to 9)
                // Assuming each set has a proper First9PPD calculated from up to 9 darts
                int dartsInThisSet = 9; // We should always use 9 darts per set for First9PPD

                // Calculate weighted points contribution
                decimal pointsFromThisSet = setStat.First9PPD * dartsInThisSet;

                totalWeightedPoints += pointsFromThisSet;
                totalDarts += dartsInThisSet;
            }

            return totalDarts > 0 ? Math.Round(totalWeightedPoints / totalDarts, 2) : 0;
        }

        public static int CalculateCheckoutPercentage(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            int totalAttempts = turns.Count(t => t.IsCheckoutAttempt);
            int successfulCheckouts = turns.Count(t => t.IsCheckoutSuccessful);
            if (totalAttempts == 0) return 0;
            return (int)Math.Round((double)successfulCheckouts / totalAttempts * 100, 0);
        }

        public static int CalculateCount60Plus(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            return turns
                .Count(t => {
                    var turnScore = CalculateTurnScore(t);
                    return turnScore >= 60 && turnScore < 100;
                });
        }

        public static int CalculateCount100Plus(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            return turns
                 .Count(t => {
                     var turnScore = CalculateTurnScore(t);
                     return turnScore >= 100 && turnScore < 140;
                 });
        }

        public static int CalculateCount140Plus(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            return turns
                 .Count(t => {
                     var turnScore = CalculateTurnScore(t);
                     return turnScore >= 140 && turnScore < 180;
                 });
        }

        public static int CalculateCount180s(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            return turns
                 .Count(t => CalculateTurnScore(t) == 180);
        }

        private static int CalculateTurnScore(Turn turn)
        {
            if (turn == null || IsNullOrEmpty(turn.Throws)) return 0;

            // Check if turn contains any busted throws
            if (turn.Throws.Any(th => th.IsBusted)) return 0;

            return turn.Throws.Sum(th => th.Multiplier * th.Segment);
        }

        private static bool IsNullOrEmpty<T>(IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        private static decimal CalculatePPDFromPointsAndThrows(decimal totalPoints, int totalThrows)
        {
            return totalThrows > 0
                ? Math.Round(totalPoints / totalThrows, 2)
                : 0;
        }
    }
}
