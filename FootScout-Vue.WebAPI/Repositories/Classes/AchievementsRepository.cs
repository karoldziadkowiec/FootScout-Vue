using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi z osgiągnięciami dla historii klubowej
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly AppDbContext _dbContext;

        public AchievementsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Tworzenie nowego osiągnięcia klubowego dla historii klubowej
        public async Task CreateAchievements(Achievements achievements)
        {
            await _dbContext.Achievements.AddAsync(achievements);
            await _dbContext.SaveChangesAsync();
        }
    }
}