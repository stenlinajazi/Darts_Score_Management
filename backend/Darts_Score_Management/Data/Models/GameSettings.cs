using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Darts_Score_Management.Data.Models
{
    public class GameSettings
    {
        //stored as a JSON property in the Game table rather than as a separate entity with its own table
        public bool MustFinishOnDouble { get; set; }
        public int SetsToWin { get; set; }
        public int LegsPerSet { get; set; }
    }
}
