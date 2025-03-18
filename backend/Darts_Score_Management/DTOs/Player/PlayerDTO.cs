namespace Darts_Score_Management.DTOs.Player
{
    public class PlayerDTO : UpsertPlayerDTO
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
