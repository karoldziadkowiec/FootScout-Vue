using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.IntegrationTests.TestManager;
using FootScout_Vue.WebAPI.Repositories.Classes;

namespace FootScout_Vue.WebAPI.IntegrationTests.Repositories
{
    public class SalaryRangeRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly SalaryRangeRepository _salaryRangeRepository;

        public SalaryRangeRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _salaryRangeRepository = new SalaryRangeRepository(_dbContext);
        }

        [Fact]
        public async Task CreateSalaryRange_AddsNewSalaryRange()
        {
            // Arrange
            var newSalaryRange = new SalaryRange
            {
                Min = 80.0,
                Max = 160.0
            };

            // Act
            await _salaryRangeRepository.CreateSalaryRange(newSalaryRange);

            // Assert
            var result = await _dbContext.SalaryRanges.FindAsync(5);
            Assert.NotNull(result);

            _dbContext.SalaryRanges.Remove(result);
            await _dbContext.SaveChangesAsync();
        }
    }
}