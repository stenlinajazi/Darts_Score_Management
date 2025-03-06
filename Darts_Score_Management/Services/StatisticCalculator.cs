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

        // Calculates First 9 Darts  (PPD) for Turns
        //Extracts the first 9 throws from all turns
        public static decimal CalculateFirst9PPD(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            var orderedTurns = turns.OrderBy(t => t.TurnNumber).ToList();
            var first9Throws = orderedTurns.SelectMany(t => t.Throws).Take(9).ToList();
            var totalPoints = first9Throws.Sum(t => t.Multiplier * t.Segment);
            var totalDarts = Math.Min(9, first9Throws.Count);
            return CalculatePPDFromPointsAndThrows(totalPoints, totalDarts);
        }

        // Calculates First 9 Darts PPD for Leg Statistics
        //Used to calculate the aggregate PPD for the first 9 darts across multiple legs
        /*
        Assumes that each LegStats object already has a precomputed First9PPD value.

        Multiplies each First9PPD by 9 (assuming 9 darts per leg) to get the total points for the first 9 darts in each leg.

        Sums up the total points across all legs and divides by the total number of darts (number of legs × 9) to get the aggregated PPD
        */
        public static decimal CalculateFirst9PPDAggregate(IEnumerable<LegStats> legStats)
        {
            if (IsNullOrEmpty(legStats)) return 0;
            var validStats = legStats.Where(ls => ls.TotalThrows > 0 && ls.First9PPD > 0).ToList();
            if (!validStats.Any()) return 0;

            // Use TotalThrows to determine the actual darts thrown, capped at 9 per leg
            decimal totalPoints = 0;
            int totalDarts = 0;
            foreach (var ls in validStats.OrderBy(ls => ls.LegId))
            {
                int dartsInLeg = Math.Min(ls.TotalThrows, 9 - totalDarts); // Cap at 9 total across legs
                if (dartsInLeg <= 0) break;
                totalPoints += ls.First9PPD * dartsInLeg; // Scale by actual throws
                totalDarts += dartsInLeg;
            }
            return CalculatePPDFromPointsAndThrows(totalPoints, totalDarts);
        }
        
        // Calculates First 9 Darts PPD for Set Statistics
        public static decimal CalculateFirst9PPDAggregate(IEnumerable<SetStats> setStats)
        {
            if (IsNullOrEmpty(setStats)) return 0;
            var validStats = setStats.Where(ss => ss.TotalThrows > 0 && ss.First9PPD > 0).ToList();
            if (!validStats.Any()) return 0;

            // Use TotalThrows to determine the actual darts thrown, capped at 9 across sets
            decimal totalPoints = 0;
            int totalDarts = 0;
            foreach (var ss in validStats.OrderBy(ss => ss.SetId))
            {
                int dartsInSet = Math.Min(ss.TotalThrows, 9 - totalDarts); // Cap at 9 total across sets
                if (dartsInSet <= 0) break;
                totalPoints += ss.First9PPD * dartsInSet; // Scale by actual throws
                totalDarts += dartsInSet;
            }
            return CalculatePPDFromPointsAndThrows(totalPoints, totalDarts);
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

        //public static decimal CalculateAverageCheckout(IEnumerable<Turn> turns, int legsWon)
        //{
        //    if (legsWon == 0) return 0;
        //    var checkouts = turns.Where(t => t.TotalPoints == 0).Select(t => t.Throws.Sum(th => th.Multiplier * th.Segment));
        //    var totalCheckout = checkouts.Sum();
        //    return checkouts.Any() ? Math.Round(totalCheckout / (decimal)legsWon, 2) : 0;
        //}

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
