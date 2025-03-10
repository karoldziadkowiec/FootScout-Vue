﻿using AutoMapper;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    [Route("api/player-offers")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class PlayerOfferController : ControllerBase
    {
        private readonly IPlayerOfferRepository _playerOfferRepository;
        private readonly IMapper _mapper;

        public PlayerOfferController(IPlayerOfferRepository playerOfferRepository, IMapper mapper)
        {
            _playerOfferRepository = playerOfferRepository;
            _mapper = mapper;
        }

        // GET: api/player-offers/:playerOfferId
        [HttpGet("{playerOfferId}")]
        public async Task<ActionResult<PlayerOffer>> GetPlayerOffer(int playerOfferId)
        {
            var playerOffer = await _playerOfferRepository.GetPlayerOffer(playerOfferId);
            if (playerOffer == null)
                return NotFound();

            return Ok(playerOffer);
        }

        // GET: api/player-offers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerOffer>>> GetPlayerOffers()
        {
            var playerOffers = await _playerOfferRepository.GetPlayerOffers();
            return Ok(playerOffers);
        }

        // GET: api/player-offers/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PlayerOffer>>> GetActivePlayerOffers()
        {
            var activePlayerOffers = await _playerOfferRepository.GetActivePlayerOffers();
            return Ok(activePlayerOffers);
        }

        // GET: api/player-offers/active/count
        [HttpGet("active/count")]
        public async Task<IActionResult> GetActivePlayerOfferCount()
        {
            int count = await _playerOfferRepository.GetActivePlayerOfferCount();
            return Ok(count);
        }

        // GET: api/player-offers/inactive
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<PlayerOffer>>> GetInactivePlayerOffers()
        {
            var inactivePlayerOffers = await _playerOfferRepository.GetInactivePlayerOffers();
            return Ok(inactivePlayerOffers);
        }

        // POST: api/player-offers
        [HttpPost]
        public async Task<ActionResult> CreatePlayerOffer([FromBody] PlayerOfferCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var playerOffer = _mapper.Map<PlayerOffer>(dto);
            await _playerOfferRepository.CreatePlayerOffer(playerOffer);

            return Ok(playerOffer);
        }

        // PUT: api/player-offers/:playerOfferId
        [HttpPut("{playerOfferId}")]
        public async Task<ActionResult> UpdatePlayerOffer(int playerOfferId, [FromBody] PlayerOffer playerOffer)
        {
            if (playerOfferId != playerOffer.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _playerOfferRepository.UpdatePlayerOffer(playerOffer);
            return NoContent();
        }

        // DELETE: api/player-offers/:playerOfferId
        [HttpDelete("{playerOfferId}")]
        public async Task<IActionResult> DeletePlayerOffer(int playerOfferId)
        {
            try
            {
                if (await _playerOfferRepository.GetPlayerOffer(playerOfferId) == null)
                    return NotFound($"Player offer : {playerOfferId} not found");

                await _playerOfferRepository.DeletePlayerOffer(playerOfferId);
            }
            catch (Exception)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT: api/player-offers/accept/:playerOfferId
        [HttpPut("accept/{playerOfferId}")]
        public async Task<ActionResult> AcceptPlayerOffer(int playerOfferId, [FromBody] PlayerOffer playerOffer)
        {
            if (playerOfferId != playerOffer.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _playerOfferRepository.AcceptPlayerOffer(playerOffer);
            return NoContent();
        }

        // PUT: api/player-offers/reject/:playerOfferId
        [HttpPut("reject/{playerOfferId}")]
        public async Task<ActionResult> RejectPlayerOffer(int playerOfferId, [FromBody] PlayerOffer playerOffer)
        {
            if (playerOfferId != playerOffer.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _playerOfferRepository.RejectPlayerOffer(playerOffer);
            return NoContent();
        }

        // GET: api/player-offers/status/:clubAdvertisementId/:userId
        [HttpGet("status/{clubAdvertisementId}/{userId}")]
        public async Task<IActionResult> GetPlayerOfferStatusId(int clubAdvertisementId, string userId)
        {
            var playerOfferStatusId = await _playerOfferRepository.GetPlayerOfferStatusId(clubAdvertisementId, userId);
            return Ok(playerOfferStatusId);
        }

        // GET: api/player-offers/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportPlayerOffersToCsv()
        {
            var csvStream = await _playerOfferRepository.ExportPlayerOffersToCsv();
            return File(csvStream, "text/csv", "player-offers.csv");
        }
    }
}