using System.ComponentModel.DataAnnotations;

namespace FootScout_Vue.WebAPI.Entities
{
    // Model (encja) widełek płacowych
    public class SalaryRange
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Min { get; set; }

        [Required]
        public double Max { get; set; }
    }
}