using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi z historiami klubowymi
    public class ClubHistoryRepository : IClubHistoryRepository
    {
        private readonly AppDbContext _dbContext;

        public ClubHistoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Zwróć konkretną historię klubową dla danego id
        public async Task<ClubHistory> GetClubHistory(int clubHistoryId)
        {
            return await _dbContext.ClubHistories
                .Include(ch=> ch.PlayerPosition)
                .Include(ch => ch.Achievements)
                .Include(ch => ch.Player)
                .FirstOrDefaultAsync(ch => ch.Id == clubHistoryId);
        }

        // Zwróć wszystkie historie klubowe
        public async Task<IEnumerable<ClubHistory>> GetAllClubHistory()
        {
            return await _dbContext.ClubHistories
                .Include(ch => ch.PlayerPosition)
                .Include(ch => ch.Achievements)
                .Include(ch => ch.Player)
                .ToListAsync();
        }

        // Zwróć liczbę wszystkich historii klubowych
        public async Task<int> GetClubHistoryCount()
        {
            return await _dbContext.ClubHistories.CountAsync();
        }

        // Utwórz nową historię klubową
        public async Task CreateClubHistory(ClubHistory clubHistory)
        {
            await _dbContext.ClubHistories.AddAsync(clubHistory);
            await _dbContext.SaveChangesAsync();
        }

        // Zaktualizuj konkretną historię klubową
        public async Task UpdateClubHistory(ClubHistory clubHistory)
        {
            _dbContext.ClubHistories.Update(clubHistory);
            await _dbContext.SaveChangesAsync();
        }

        // Usuń konkrentą historię klubową
        public async Task DeleteClubHistory(int clubHistoryId)
        {
            var clubHistory = await _dbContext.ClubHistories.FindAsync(clubHistoryId);
            if (clubHistory == null)
                throw new ArgumentException($"No club history found with ID {clubHistoryId}");

            if (clubHistory.AchievementsId != null)
            {
                var achievements = await _dbContext.Achievements.FindAsync(clubHistory.AchievementsId);
                if (achievements != null)
                    _dbContext.Achievements.Remove(achievements);
            }

            _dbContext.ClubHistories.Remove(clubHistory);
            await _dbContext.SaveChangesAsync();
        }
    }
}