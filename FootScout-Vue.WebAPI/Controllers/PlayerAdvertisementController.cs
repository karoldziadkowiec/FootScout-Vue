using AutoMapper;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    // Kontroler API dla ogłoszenia piłkarza
    [Route("api/player-advertisements")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class PlayerAdvertisementController : ControllerBase
    {
        private readonly IPlayerAdvertisementRepository _playerAdvertisementRepository;
        private readonly ISalaryRangeRepository _salaryRangeRepository;
        private readonly IMapper _mapper;

        public PlayerAdvertisementController(IPlayerAdvertisementRepository playerAdvertisementRepository, ISalaryRangeRepository salaryRangeRepository, IMapper mapper)
        {
            _playerAdvertisementRepository = playerAdvertisementRepository;
            _salaryRangeRepository = salaryRangeRepository;
            _mapper = mapper;
        }

        // GET: api/player-advertisements/:playerAdvertisementId  ->  zwróć ogłoszenie dla konkretnego id
        [HttpGet("{playerAdvertisementId}")]
        public async Task<ActionResult<PlayerAdvertisement>> GetPlayerAdvertisement(int playerAdvertisementId)
        {
            var playerAdvertisement = await _playerAdvertisementRepository.GetPlayerAdvertisement(playerAdvertisementId);
            if (playerAdvertisement == null)
                return NotFound();

            return Ok(playerAdvertisement);
        }

        // GET: api/player-advertisements  ->  zwróć wszystkie ogłoszenia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetAllPlayerAdvertisements()
        {
            var playerAdvertisements = await _playerAdvertisementRepository.GetAllPlayerAdvertisements();
            return Ok(playerAdvertisements);
        }

        // GET: api/player-advertisements/active  ->  zwróć aktywne ogłoszenia
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetActivePlayerAdvertisements()
        {
            var activePlayerAdvertisements = await _playerAdvertisementRepository.GetActivePlayerAdvertisements();
            return Ok(activePlayerAdvertisements);
        }

        // GET: api/player-advertisements/active/count  ->  zwróć liczbę aktywnych ogłoszeń
        [HttpGet("active/count")]
        public async Task<IActionResult> GetActivePlayerAdvertisementCount()
        {
            int count = await _playerAdvertisementRepository.GetActivePlayerAdvertisementCount();
            return Ok(count);
        }

        // GET: api/player-advertisements/inactive  ->  zwróć nieaktywne ogłoszenia
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetInactivePlayerAdvertisements()
        {
            var inactivePlayerAdvertisements = await _playerAdvertisementRepository.GetInactivePlayerAdvertisements();
            return Ok(inactivePlayerAdvertisements);
        }

        // POST: api/player-advertisements  ->  utwórz nowe ogłoszenie
        [HttpPost]
        public async Task<ActionResult> CreatePlayerAdvertisement([FromBody] PlayerAdvertisementCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var salaryRange = _mapper.Map<SalaryRange>(dto.SalaryRangeDTO);
            await _salaryRangeRepository.CreateSalaryRange(salaryRange);

            var playerAdvertisement = _mapper.Map<PlayerAdvertisement>(dto);
            playerAdvertisement.SalaryRangeId = salaryRange.Id;
            await _playerAdvertisementRepository.CreatePlayerAdvertisement(playerAdvertisement);

            return Ok(playerAdvertisement);
        }

        // PUT: api/player-advertisements/:playerAdvertisementId  ->  zaktualizuj konkretne ogłoszenie
        [HttpPut("{playerAdvertisementId}")]
        public async Task<ActionResult> UpdatePlayerAdvertisement(int playerAdvertisementId, [FromBody] PlayerAdvertisement playerAdvertisement)
        {
            if (playerAdvertisementId != playerAdvertisement.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _playerAdvertisementRepository.UpdatePlayerAdvertisement(playerAdvertisement);
            return NoContent();
        }

        // DELETE: api/player-advertisements/:playerAdvertisementId  ->  usuń konkretne ogłoszenie
        [HttpDelete("{playerAdvertisementId}")]
        public async Task<ActionResult> DeletePlayerAdvertisement(int playerAdvertisementId)
        {
            var playerAdvertisement = await _playerAdvertisementRepository.GetPlayerAdvertisement(playerAdvertisementId);
            if (playerAdvertisement == null)
                return NotFound();

            await _playerAdvertisementRepository.DeletePlayerAdvertisement(playerAdvertisementId);
            return NoContent();
        }

        // GET: api/player-advertisements/export  ->  eksportuj ogłoszenia do pliku .csv
        [HttpGet("export")]
        public async Task<IActionResult> ExportPlayerAdvertisementsToCsv()
        {
            var csvStream = await _playerAdvertisementRepository.ExportPlayerAdvertisementsToCsv();
            return File(csvStream, "text/csv", "player-advertisements.csv");
        }
    }
}