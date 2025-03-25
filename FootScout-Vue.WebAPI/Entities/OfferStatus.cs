using System.ComponentModel.DataAnnotations;

namespace FootScout_Vue.WebAPI.Entities
{
    // Model (encja) statusu oferty
    public class OfferStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string StatusName { get; set; }
    }
}