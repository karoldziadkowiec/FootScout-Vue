using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    public interface IPlayerFootRepository
    {
        Task<IEnumerable<PlayerFoot>> GetPlayerFeet();
        Task<string> GetPlayerFootName(int footId);
    }
}