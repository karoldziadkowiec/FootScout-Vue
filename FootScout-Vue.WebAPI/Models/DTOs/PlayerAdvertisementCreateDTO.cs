using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla tworzenia ogłoszenia piłkarskiego
    public class PlayerAdvertisementCreateDTO
    {
        public int PlayerPositionId { get; set; }
        public string League { get; set; }
        public string Region { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int PlayerFootId { get; set; }
        public SalaryRangeDTO SalaryRangeDTO { get; set; }
        public string PlayerId { get; set; }
    }
}