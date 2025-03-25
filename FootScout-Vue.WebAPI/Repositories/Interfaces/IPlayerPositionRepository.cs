using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    // Interfejs deklarujący operacje związane z pozycjami piłkarskimi
    public interface IPlayerPositionRepository
    {
        Task<IEnumerable<PlayerPosition>> GetPlayerPositions();
        Task<int> GetPlayerPositionCount();
        Task<string> GetPlayerPositionName(int positionId);
        Task<bool> CheckPlayerPositionExists(string positionName);
        Task CreatePlayerPosition(PlayerPosition playerPosition);
    }
}