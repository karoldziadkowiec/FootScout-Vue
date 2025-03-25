using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi z problemami aplikacji
    public class ProblemRepository : IProblemRepository
    {
        private readonly AppDbContext _dbContext;

        public ProblemRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Zwróć konkretny problem
        public async Task<Problem> GetProblem(int problemId)
        {
            return await _dbContext.Problems
                .Include(p => p.Requester)
                .FirstOrDefaultAsync(p => p.Id == problemId);
        }

        // Zwróć wszystkie problemy
        public async Task<IEnumerable<Problem>> GetAllProblems()
        {
            return await _dbContext.Problems
                .Include(p => p.Requester)
                .OrderByDescending(p => p.CreationDate)
                .ToListAsync();
        }

        // Zwróć wszystkie rozwiązane problemy
        public async Task<IEnumerable<Problem>> GetSolvedProblems()
        {
            return await _dbContext.Problems
                .Include(p => p.Requester)
                .Where(p => p.IsSolved == true)
                .OrderByDescending(p => p.CreationDate)
                .ToListAsync();
        }

        // Zwróć liczbę wszystkich rozwiązanych problemów
        public async Task<int> GetSolvedProblemCount()
        {
            return await _dbContext.Problems.Where(p => p.IsSolved == true).CountAsync();
        }

        // Zwróć wszystkie nierozwiązane problemy
        public async Task<IEnumerable<Problem>> GetUnsolvedProblems()
        {
            return await _dbContext.Problems
                .Include(p => p.Requester)
                .Where(pa => pa.IsSolved == false)
                .OrderByDescending(p => p.CreationDate)
                .ToListAsync();
        }

        // Zwróć liczbę wszystkich nierozwiązanych problemów
        public async Task<int> GetUnsolvedProblemCount()
        {
            return await _dbContext.Problems.Where(p => p.IsSolved == false).CountAsync();
        }

        // Utwórz nowy problem
        public async Task CreateProblem(Problem problem)
        {
            problem.CreationDate = DateTime.Now;
            problem.IsSolved = false;

            await _dbContext.Problems.AddAsync(problem);
            await _dbContext.SaveChangesAsync();
        }

        // Oznacz konkretny problem jako rozwiązany
        public async Task CheckProblemSolved(Problem problem)
        {
            problem.IsSolved = true;

            _dbContext.Problems.Update(problem);
            await _dbContext.SaveChangesAsync();
        }

        // Eksportuj problem do pliku .csv
        public async Task<MemoryStream> ExportProblemsToCsv()
        {
            var problems = await GetAllProblems();
            var csv = new StringBuilder();
            csv.AppendLine("Problem Id,Is Solved,Requester E-mail,Requester First Name,Requester Last Name,Title,Description,Creation Date");

            foreach (var problem in problems)
            {
                csv.AppendLine($"{problem.Id},{problem.IsSolved},{problem.Requester.Email},{problem.Requester.FirstName},{problem.Requester.LastName},{problem.Title},{problem.Description},{problem.CreationDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}