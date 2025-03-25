using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi z widełkami płacowymi
    public class SalaryRangeRepository : ISalaryRangeRepository
    {
        private readonly AppDbContext _dbContext;

        public SalaryRangeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        // Utwórz nowe widełki płacowe
        public async Task CreateSalaryRange(SalaryRange salaryRange)
        {
            await _dbContext.SalaryRanges.AddAsync(salaryRange);
            await _dbContext.SaveChangesAsync();
        }
    }
}