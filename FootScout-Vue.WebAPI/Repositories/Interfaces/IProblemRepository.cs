using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    // Interfejs deklarujący operacje związane z problemami aplikacji
    public interface IProblemRepository
    {
        Task<Problem> GetProblem(int playerAdvertisementId);
        Task<IEnumerable<Problem>> GetAllProblems();
        Task<IEnumerable<Problem>> GetSolvedProblems();
        Task<int> GetSolvedProblemCount();
        Task<IEnumerable<Problem>> GetUnsolvedProblems();
        Task<int> GetUnsolvedProblemCount();
        Task CreateProblem(Problem problem);
        Task CheckProblemSolved(Problem problem);
        Task<MemoryStream> ExportProblemsToCsv();
    }
}