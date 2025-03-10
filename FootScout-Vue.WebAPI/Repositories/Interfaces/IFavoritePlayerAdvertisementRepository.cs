using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    public interface IFavoritePlayerAdvertisementRepository
    {
        Task AddToFavorites(FavoritePlayerAdvertisement favoritePlayerAdvertisement);
        Task DeleteFromFavorites(int favoritePlayerAdvertisementId);
        Task<int> CheckPlayerAdvertisementIsFavorite(int playerAdvertisementId, string userId);
    }
}