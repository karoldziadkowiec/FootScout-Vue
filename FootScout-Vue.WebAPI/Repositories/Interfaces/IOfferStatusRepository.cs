using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Repositories.Interfaces
{
    // Interfejs deklarujący operacje związane z statusami ofert
    public interface IOfferStatusRepository
    {
        Task<IEnumerable<OfferStatus>> GetOfferStatuses();
        Task<OfferStatus> GetOfferStatus(int offerStatusId);
        Task<string> GetOfferStatusName(int offerStatusId);
        Task<int> GetOfferStatusId(string offerStatusName);
    }
}