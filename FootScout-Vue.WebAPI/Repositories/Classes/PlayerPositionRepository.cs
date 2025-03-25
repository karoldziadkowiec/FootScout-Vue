using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi z pozycjami piłkarskimi
    public class PlayerPositionRepository : IPlayerPositionRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerPositionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Zwróć wszystkie pozycje piłkarskie
        public async Task<IEnumerable<PlayerPosition>> GetPlayerPositions()
            => await _dbContext.PlayerPositions.ToListAsync();

        // Zwróć liczbę wszystkich pozycji piłkarskich
        public async Task<int> GetPlayerPositionCount()
            => await _dbContext.PlayerPositions.CountAsync();

        // Zwróć nazwę dla konkretnego id pozycji piłkarskiej
        public async Task<string> GetPlayerPositionName(int positionId)
            => await _dbContext.PlayerPositions.Where(p => p.Id == positionId).Select(p => p.PositionName).FirstOrDefaultAsync();

        // Sprawdź czy konkretna pozycja piłkarska isniteje
        public async Task<bool> CheckPlayerPositionExists(string positionName)
        {
            return await _dbContext.PlayerPositions
                .AnyAsync(p => p.PositionName == positionName);
        }

        // Utwórz nową pozycję piłkarską
        public async Task CreatePlayerPosition(PlayerPosition playerPosition)
        {
            var isExists = await _dbContext.PlayerPositions.AnyAsync(p => p.PositionName == playerPosition.PositionName);
            if (isExists == true)
                throw new ArgumentException($"Position {playerPosition.PositionName} already exists!");

            await _dbContext.PlayerPositions.AddAsync(playerPosition);
            await _dbContext.SaveChangesAsync();
        }
    }
}