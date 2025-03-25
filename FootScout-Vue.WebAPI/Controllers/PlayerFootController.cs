using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    // Kontroler API dla nóg piłkarzy
    [Route("api/player-feet")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class PlayerFootController : ControllerBase
    {
        private readonly IPlayerFootRepository _playerFootRepository;

        public PlayerFootController(IPlayerFootRepository playerFootRepository)
        {
            _playerFootRepository = playerFootRepository;
        }

        // GET: api/player-feet  ->  zwróć nogi piłkarza
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerFoot>>> GetPlayerFeet()
        {
            var playerFeet = await _playerFootRepository.GetPlayerFeet();
            return Ok(playerFeet);
        }

        // GET: api/player-feet/:footId  ->  zwróć nazwę nogi dla konkretnego id
        [HttpGet("{footId}")]
        public async Task<IActionResult> GetPlayerFootName(int footId)
        {
            var footName = await _playerFootRepository.GetPlayerFootName(footId);
            if (footName == null)
                return NotFound();

            return Ok(footName);
        }
    }
}