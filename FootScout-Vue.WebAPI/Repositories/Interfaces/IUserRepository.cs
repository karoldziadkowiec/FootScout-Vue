using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    // Interfejs deklarujący operacje związane z użytkownikiem
    public interface IUserRepository
    {
        Task<UserDTO> GetUser(string userId);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<IEnumerable<UserDTO>> GetOnlyUsers();
        Task<IEnumerable<UserDTO>> GetOnlyAdmins();
        Task<string> GetUserRole(string userId);
        Task<int> GetUserCount();
        Task UpdateUser(string userId, UserUpdateDTO userUpdateDTO);
        Task ResetUserPassword(string userId, UserResetPasswordDTO userUpdateDTO);
        Task DeleteUser(string userId);
        Task<IEnumerable<ClubHistory>> GetUserClubHistory(string userId);
        Task<IEnumerable<PlayerAdvertisement>> GetUserPlayerAdvertisements(string userId);
        Task<IEnumerable<PlayerAdvertisement>> GetUserActivePlayerAdvertisements(string userId);
        Task<IEnumerable<PlayerAdvertisement>> GetUserInactivePlayerAdvertisements(string userId);
        Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserFavoritePlayerAdvertisements(string userId);
        Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserActiveFavoritePlayerAdvertisements(string userId);
        Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserInactiveFavoritePlayerAdvertisements(string userId);
        Task<IEnumerable<ClubOffer>> GetReceivedClubOffers(string userId);
        Task<IEnumerable<ClubOffer>> GetSentClubOffers(string userId);
        Task<IEnumerable<Chat>> GetUserChats(string userId);
        Task<MemoryStream> ExportUsersToCsv();
    }
}