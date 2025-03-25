using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    // Interfejs deklarujący operacje związane z osiągnięciami zawodnika dla historii klubowych
    public interface IAchievementsRepository
    {
        Task CreateAchievements(Achievements achievements);
    }
}