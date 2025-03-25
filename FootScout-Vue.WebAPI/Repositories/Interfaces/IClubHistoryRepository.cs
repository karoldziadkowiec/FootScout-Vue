using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    // Interfejs deklarujący operacje związane z historiami klubowymi
    public interface IClubHistoryRepository
    {
        Task<ClubHistory> GetClubHistory(int clubHistoryId);
        Task<IEnumerable<ClubHistory>> GetAllClubHistory();
        Task<int> GetClubHistoryCount();
        Task CreateClubHistory(ClubHistory clubHistory);
        Task UpdateClubHistory(ClubHistory clubHistory);
        Task DeleteClubHistory(int clubHistoryId);
    }
}