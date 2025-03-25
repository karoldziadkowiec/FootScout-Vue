using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    // Interfejs deklarujący operacje związane z widełkami płacowymi
    public interface ISalaryRangeRepository
    {
        Task CreateSalaryRange(SalaryRange salaryRange);
    }
}