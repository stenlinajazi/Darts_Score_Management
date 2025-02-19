namespace Darts_Score_Management.DTOs.Throw
{
    public class ThrowDTO
    {
        public int Id { get; set; }
        public int TurnId { get; set; }
        public int ThrowNumber { get; set; }
        public int Segment { get; set; }
        public int Multiplier { get; set; }
        public bool IsBusted { get; set; }
        public int Points { get; set; }
    }
}
