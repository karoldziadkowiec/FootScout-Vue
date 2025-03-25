using System.ComponentModel.DataAnnotations;

namespace FootScout_Vue.WebAPI.Entities
{
    // Model (encja) nogi piłkarza
    public class PlayerFoot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string FootName { get; set; }
    }
}