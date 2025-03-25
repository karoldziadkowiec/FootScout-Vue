using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    // Kontroler API dla pozycji piłkarskich
    [Route("api/player-positions")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class PlayerPositionController : ControllerBase
    {
        private readonly IPlayerPositionRepository _playerPositionRepository;

        public PlayerPositionController(IPlayerPositionRepository playerPositionRepository)
        {
            _playerPositionRepository = playerPositionRepository;
        }

        // GET: api/player-positions  ->  zwróć wszystkie pozycje piłkarskie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerPosition>>> GetPlayerPositions()
        {
            var playerPositions = await _playerPositionRepository.GetPlayerPositions();
            return Ok(playerPositions);
        }

        // GET: api/player-positions/count  ->  zwróć liczbę wszystkich pozycji piłkarskich
        [HttpGet("count")]
        public async Task<IActionResult> GetPlayerPositionCount()
        {
            int count = await _playerPositionRepository.GetPlayerPositionCount();
            return Ok(count);
        }

        // GET: api/player-positions/:positionId  ->  zwróć nazwę dla konkretnego id pozycji piłkarskiej
        [HttpGet("{positionId}")]
        public async Task<IActionResult> GetPlayerPositionName(int positionId)
        {
            var positionName = await _playerPositionRepository.GetPlayerPositionName(positionId);
            if (positionName == null)
                return NotFound();

            return Ok(positionName);
        }

        // GET: api/player-positions/check/name/:positionName  ->  sprawdź czy pozycja piłkarska istnieje bazując na nazwie - jeżeli tak to zwróć jego id, jeżeli nie to zwróć 0
        [HttpGet("check/name/{positionName}")]
        public async Task<IActionResult> CheckPlayerPositionExists(string positionName)
        {
            var isExists = await _playerPositionRepository.CheckPlayerPositionExists(positionName);
            return Ok(isExists);
        }

        // POST: api/player-positions  ->  utwórz nową pozycję piłarską
        [HttpPost]
        public async Task<ActionResult> CreatePlayerPosition([FromBody] PlayerPosition playerPosition)
        {
            if (playerPosition == null)
                return BadRequest("Invalid player position data.");

            await _playerPositionRepository.CreatePlayerPosition(playerPosition);
            return Ok(playerPosition);
        }
    }
}