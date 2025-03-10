using System.ComponentModel.DataAnnotations;

namespace FootScout_Vue.WebAPI.Entities
{
    public class PlayerPosition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string PositionName { get; set; }
    }
}