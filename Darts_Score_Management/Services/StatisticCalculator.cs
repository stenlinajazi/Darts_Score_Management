using Darts_Score_Management.Data.Models;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Darts_Score_Management.Services
{
    public static class StatisticCalculator
    {
        public static decimal CalculatePPD(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            var totalPoints = turns.Sum(t => t.TotalPoints);
            var totalDarts = turns.Sum(t => t.Throws.Count);
            return CalculatePPDFromPointsAndThrows(totalPoints, totalDarts);
        } 
        
        public static decimal CalculatePPDAggregate(IEnumerable<LegStats> legStats)
        {
            if (IsNullOrEmpty(legStats)) return 0;
            var totalPoints = legStats.Sum(ls => ls.PPD * ls.TotalThrows);
            var totalThrows = legStats.Sum(ls => ls.TotalThrows);
            return CalculatePPDFromPointsAndThrows(totalPoints, totalThrows);
        }
        
        public static decimal CalculatePPDAggregate(IEnumerable<SetStats> setStats)
        {
            if (IsNullOrEmpty(setStats)) return 0;
            var totalPoints = setStats.Sum(ss => ss.PPD * ss.TotalThrows);
            var totalThrows = setStats.Sum(ss => ss.TotalThrows);
            return CalculatePPDFromPointsAndThrows(totalPoints, totalThrows);
        }

        public static decimal CalculateFirst9PPD(IEnumerable<Turn> turns)
        {
            if (IsNullOrEmpty(turns)) return 0;
            var orderedTurns = turns.OrderBy(t => t.TurnNumber).ToList();

            decimal totalPoints = 0;
            int dartCount = 0;

            foreach (var turn in orderedTurns)
            {
                bool turnIsBusted = turn.Throws.Any(th => th.IsBusted);

                int turnsThrowCount = turn.Throws.Count;
                int dartsToCount = Math.Min(turnsThrowCount, 9 - dartCount);

                if (dartsToCount <= 0) break;
 
                if (!turnIsBusted)
                {
                   
                    var throwPoints = turn.Throws
                        .Take(dartsToCount)
                        .Sum(th => th.Multiplier * th.Segment);

                    totalPoints += throwPoints;
                }

                dartCount += dartsToCount;

                if (dartCount >= 9) break;
            }

            return CalculatePPDFromPointsAndThrows(totalPoints, dartCount);
        }

        public static decimal CalculateFirst9PPDAggregate(IEnumerable<LegStats> legStats)
        {
            if (IsNullOrEmpty(legStats)) return 0;

            decimal totalWeightedPoints = 0;
            int totalDarts = 0;

     
            foreach (var legStat in legStats)
            {
                int dartsInThisLeg = legStat.TotalThrows >= 9 ? 9 : legStat.TotalThrows;

           
                decimal pointsFromThisLeg = legStat.First9PPD * dartsInThisLeg;

                totalWeightedPoints += pointsFromThisLeg;
                totalDarts += dartsInThisLeg;
            }

            return totalDarts > 0 ? Math.Round(totalWeightedPoints / totalDarts, 2) : 0;
        }

  
        public static decimal CalculateFirst9PPDAggregate(IEnumerable<SetStats> setStats)
        {
            if (IsNullOrEmpty(setStats)) return 0;

            decimal totalWeightedPoints = 0;
            int totalDarts = 0;

            foreach (var setStat in setStats)
            {
                int dartsInThisSet = 9; 

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
