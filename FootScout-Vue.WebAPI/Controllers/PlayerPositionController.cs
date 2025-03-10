﻿using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
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

        // GET: api/player-positions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerPosition>>> GetPlayerPositions()
        {
            var playerPositions = await _playerPositionRepository.GetPlayerPositions();
            return Ok(playerPositions);
        }

        // GET: api/player-positions/count
        [HttpGet("count")]
        public async Task<IActionResult> GetPlayerPositionCount()
        {
            int count = await _playerPositionRepository.GetPlayerPositionCount();
            return Ok(count);
        }

        // GET: api/player-positions/:positionId
        [HttpGet("{positionId}")]
        public async Task<IActionResult> GetPlayerPositionName(int positionId)
        {
            var positionName = await _playerPositionRepository.GetPlayerPositionName(positionId);
            if (positionName == null)
                return NotFound();

            return Ok(positionName);
        }

        // GET: api/player-positions/check/name/:positionName
        [HttpGet("check/name/{positionName}")]
        public async Task<IActionResult> CheckPlayerPositionExists(string positionName)
        {
            var isExists = await _playerPositionRepository.CheckPlayerPositionExists(positionName);
            return Ok(isExists);
        }

        // POST: api/player-positions
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