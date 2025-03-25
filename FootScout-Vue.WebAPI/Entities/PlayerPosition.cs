using System.ComponentModel.DataAnnotations;

namespace FootScout_Vue.WebAPI.Entities
{
    // Model (encja) pozycji piłkarskiej
    public class PlayerPosition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string PositionName { get; set; }
    }
}