namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla tworzenia ulubionego ogłoszenia piłkarskiego
    public class FavoritePlayerAdvertisementCreateDTO
    {
        public int PlayerAdvertisementId { get; set; }
        public string UserId { get; set; }
    }
}