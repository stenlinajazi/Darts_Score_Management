using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.Data.Models.Bases
{
    public class BaseModel
    {
        public BaseModel()
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = "System";
            ModifiedBy = string.Empty;
            DeletedBy = string.Empty;
            IsDeleted = false;
        }

        [Required]
        public DateTime CreatedAt { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; } 
       
        public DateTime? ModifiedAt { get; set; }

        [MaxLength(50)]
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [MaxLength(50)]
        public string DeletedBy { get; set; }

        
    }
}
