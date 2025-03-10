using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    public interface IFavoriteClubAdvertisementRepository
    {
        Task AddToFavorites(FavoriteClubAdvertisement favoriteClubAdvertisement);
        Task DeleteFromFavorites(int favoriteClubAdvertisementId);
        Task<int> CheckClubAdvertisementIsFavorite(int clubAdvertisementId, string userId);
    }
}