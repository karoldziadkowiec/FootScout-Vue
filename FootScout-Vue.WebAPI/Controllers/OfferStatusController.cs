using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    // Kontroler API dla oferty klubowej
    [Route("api/offer-statuses")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class OfferStatusController : ControllerBase
    {
        private readonly IOfferStatusRepository _offerStatusRepository;

        public OfferStatusController(IOfferStatusRepository offerStatusRepository)
        {
            _offerStatusRepository = offerStatusRepository;
        }

        // GET: api/offer-statuses  ->  zwróć wszystkie statusy ofert
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfferStatus>>> GetOfferStatuses()
        {
            var offerStatuses = await _offerStatusRepository.GetOfferStatuses();
            return Ok(offerStatuses);
        }

        // GET: api/offer-statuses/:offerStatusId  ->  zwróć status oferty dla konkrentego id
        [HttpGet("{offerStatusId}")]
        public async Task<IActionResult> GetOfferStatus(int offerStatusId)
        {
            var offerStatus = await _offerStatusRepository.GetOfferStatus(offerStatusId);
            if (offerStatus == null)
                return NotFound();

            return Ok(offerStatus);
        }

        // GET: api/offer-statuses/name/:statusId  ->  zwróć nazwę statusu oferty dla konkretnego id
        [HttpGet("name/{statusId}")]
        public async Task<IActionResult> GetOfferStatusName(int statusId)
        {
            var statusName = await _offerStatusRepository.GetOfferStatusName(statusId);
            if (statusName == null)
                return NotFound();

            return Ok(statusName);
        }

        // GET: api/offer-statuses/id/:statusName  ->  zwróć id statusu oferty dla konkretnej nazwy
        [HttpGet("id/{statusName}")]
        public async Task<IActionResult> GetOfferStatusId(string statusName)
        {
            var statusId = await _offerStatusRepository.GetOfferStatusId(statusName);
            if (statusId == 0)
                return NotFound();

            return Ok(statusId);
        }
    }
}