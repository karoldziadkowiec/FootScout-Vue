using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    public interface ISalaryRangeRepository
    {
        Task CreateSalaryRange(SalaryRange salaryRange);
    }
}