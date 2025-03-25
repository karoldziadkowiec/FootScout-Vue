using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi z nogami piłkarzy
    public class PlayerFootRepository : IPlayerFootRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerFootRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Zwróć wszystkie nogi piłkarzy
        public async Task<IEnumerable<PlayerFoot>> GetPlayerFeet()
            => await _dbContext.PlayerFeet.ToListAsync();

        // Zwróc nazwę nogi dla konkretnego id
        public async Task<string> GetPlayerFootName(int footId)
            => await _dbContext.PlayerFeet.Where(p => p.Id == footId).Select(p => p.FootName).FirstOrDefaultAsync();
    }
}