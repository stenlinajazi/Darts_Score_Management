namespace Darts_Score_Management.Enums
{
    public enum StatisticType
    {
        //AveragePerTurn = 1,
        //CheckoutPercentage = 2,
        //HighestScore = 3,
        //NumberOfT20 = 4,

        PPD,                // Points Per Dart
        First9PPD,          // First 9 darts PPD
        CheckoutPercentage, // Checkout success rate (already exists, but included for clarity)
        Count60Plus,        // Number of 60+ scores
        Count100Plus,       // Number of 100+ scores (added)
        Count140Plus,       // Number of 140+ scores
        Count180s,          // Number of 180s
        TotalThrows,        // Total number of throws
        HighestCheckout,    // Highest checkout achieved
        AverageCheckout     // Average checkout score
    }
}
