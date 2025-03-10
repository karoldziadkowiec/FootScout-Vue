namespace FootScout_Vue.WebAPI.Services.Interfaces
{
    public interface IPerformanceTestsService
    {
        Task SeedComponents(int testCounter);
        Task ClearDatabaseOfSeededComponents();
    }
}