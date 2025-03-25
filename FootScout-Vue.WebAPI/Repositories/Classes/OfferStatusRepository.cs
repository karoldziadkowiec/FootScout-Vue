using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi ze statusami ofert
    public class OfferStatusRepository : IOfferStatusRepository
    {
        private readonly AppDbContext _dbContext;

        public OfferStatusRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Zwróć statusy ofert
        public async Task<IEnumerable<OfferStatus>> GetOfferStatuses()
            => await _dbContext.OfferStatuses.ToListAsync();

        // Zwróć konkretny status oferty
        public async Task<OfferStatus> GetOfferStatus(int offerStatusId)
        {
            return await _dbContext.OfferStatuses.FindAsync(offerStatusId);
        }

        // Zwróć nazwę statusu oferty dla konkretnego id
        public async Task<string> GetOfferStatusName(int statusId)
            => await _dbContext.OfferStatuses.Where(a => a.Id == statusId).Select(a => a.StatusName).FirstOrDefaultAsync();

        // Zwróć id statusu oferty dla konkretnej nazwy
        public async Task<int> GetOfferStatusId(string statusName)
            => await _dbContext.OfferStatuses.Where(a => a.StatusName == statusName).Select(a => a.Id).FirstOrDefaultAsync();
    }
}