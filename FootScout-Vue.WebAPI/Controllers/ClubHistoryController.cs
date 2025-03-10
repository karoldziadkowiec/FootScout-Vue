﻿using AutoMapper;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    [Route("api/club-history")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ClubHistoryController : ControllerBase
    {
        private readonly IClubHistoryRepository _clubHistoryRepository;
        private readonly IAchievementsRepository _achievementsRepository;
        private readonly IMapper _mapper;

        public ClubHistoryController(IClubHistoryRepository clubHistoryRepository, IAchievementsRepository achievementsRepository, IMapper mapper)
        {
            _clubHistoryRepository = clubHistoryRepository;
            _achievementsRepository = achievementsRepository;
            _mapper = mapper;
        }

        // GET: api/club-history/:clubHistoryId
        [HttpGet("{clubHistoryId}")]
        public async Task<ActionResult<ClubHistory>> GetClubHistory(int clubHistoryId)
        {
            var clubHistory = await _clubHistoryRepository.GetClubHistory(clubHistoryId);
            if (clubHistory == null)
                return NotFound();

            return Ok(clubHistory);
        }

        // GET: api/club-history
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubHistory>>> GetAllClubHistory()
        {
            var clubHistories = await _clubHistoryRepository.GetAllClubHistory();
            return Ok(clubHistories);
        }

        // GET: api/club-history/count
        [HttpGet("count")]
        public async Task<IActionResult> GetClubHistoryCount()
        {
            int count = await _clubHistoryRepository.GetClubHistoryCount();
            return Ok(count);
        }

        // POST: api/club-history
        [HttpPost]
        public async Task<ActionResult> CreateClubHistory([FromBody] ClubHistoryCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            var achievements = _mapper.Map<Achievements>(dto.Achievements);
            await _achievementsRepository.CreateAchievements(achievements);

            var clubHistory = _mapper.Map<ClubHistory>(dto);
            clubHistory.AchievementsId = achievements.Id;
            await _clubHistoryRepository.CreateClubHistory(clubHistory);

            return Ok(clubHistory);
        }

        // PUT: api/club-history/:clubHistoryId
        [HttpPut("{clubHistoryId}")]
        public async Task<ActionResult> UpdateClubHistory(int clubHistoryId, [FromBody] ClubHistory clubHistory)
        {
            if (clubHistoryId != clubHistory.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _clubHistoryRepository.UpdateClubHistory(clubHistory);
            return NoContent();
        }

        // DELETE: api/club-history/:clubHistoryId
        [HttpDelete("{clubHistoryId}")]
        public async Task<ActionResult> DeleteClubHistory(int clubHistoryId)
        {
            var clubHistory = await _clubHistoryRepository.GetClubHistory(clubHistoryId);
            if (clubHistory == null)
                return NotFound();

            await _clubHistoryRepository.DeleteClubHistory(clubHistoryId);
            return NoContent();
        }
    }
}