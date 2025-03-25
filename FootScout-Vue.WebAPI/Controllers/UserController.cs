using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.Controllers
{
    // Kontroler API dla użytkowników
    [Route("api/users")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/users/:userId  ->  zwróć użytkownika dla konkretnego id
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var userDTO = await _userRepository.GetUser(userId);
            if (userDTO == null)
                return NotFound();

            return Ok(userDTO);
        }

        // GET: api/users  ->  zwróć wszystkich użytkowników
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var userDTOs = await _userRepository.GetUsers();
            return Ok(userDTOs);
        }

        // GET: api/users/role/user  ->  zwróć użytkowników dla roli User
        [HttpGet("role/user")]
        public async Task<IActionResult> GetOnlyUsers()
        {
            var onlyUserDTOs = await _userRepository.GetOnlyUsers();
            return Ok(onlyUserDTOs);
        }

        // GET: api/users/role/admin  ->  zwróć użytkowników dla roli Admin
        [HttpGet("role/admin")]
        public async Task<IActionResult> GetOnlyAdmins()
        {
            var onlyAdminDTOs = await _userRepository.GetOnlyAdmins();
            return Ok(onlyAdminDTOs);
        }

        // GET: api/users/:userId/role  ->  zwróć rolę dla konkretnego użytkownika
        [HttpGet("{userId}/role")]
        public async Task<IActionResult> GetUserRole(string userId)
        {
            string role = await _userRepository.GetUserRole(userId);
            return Ok(role);
        }

        // GET: api/users/count  ->  zwróć liczbę wszystkich użytkowników
        [HttpGet("count")]
        public async Task<IActionResult> GetUserCount()
        {
            int count = await _userRepository.GetUserCount();
            return Ok(count);
        }

        // PUT: api/users/:userId  ->  zaktualizuj konkretnego użytkownika
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody]UserUpdateDTO dto)
        {
            try
            {
                await _userRepository.UpdateUser(userId, dto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _userRepository.GetUser(userId) == null)
                    return NotFound($"User {userId} not found");
                else
                    throw;
            }
            return NoContent();
        }

        // PUT: api/users/reset-password/:userId  ->  zresetuj hasło konkretnego użytkownika
        [HttpPut("reset-password/{userId}")]
        public async Task<IActionResult> ResetUserPassword(string userId, [FromBody] UserResetPasswordDTO dto)
        {
            try
            {
                await _userRepository.ResetUserPassword(userId, dto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _userRepository.GetUser(userId) == null)
                    return NotFound($"User {userId} not found");
                else
                    throw;
            }
            return NoContent();
        }

        // DELETE: api/users/:userId  ->  usuń konkretnego uzytkownika
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var user = await _userRepository.GetUser(userId);
                if (user == null)
                    return NotFound($"User {userId} not found");

                await _userRepository.DeleteUser(userId);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.InnerException?.Message ?? ex.Message}");
            }
            return NoContent();
        }

        // GET: api/users/:userId/club-history  ->  zwróć historie klubowe dla konkretnego użytkownika
        [HttpGet("{userId}/club-history")]
        public async Task<ActionResult<IEnumerable<ClubHistory>>> GetUserClubHistory(string userId)
        {
            var userClubHistories = await _userRepository.GetUserClubHistory(userId);
            return Ok(userClubHistories);
        }

        // GET: api/users/:userId/player-advertisements  ->  zwróć ogłoszenia dla konkretnego użytkownika
        [HttpGet("{userId}/player-advertisements")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetUserPlayerAdvertisements(string userId)
        {
            var userPlayerAdvertisements = await _userRepository.GetUserPlayerAdvertisements(userId);
            return Ok(userPlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/active  ->  zwróć aktywne ogłoszenia dla konkretnego użytkownika
        [HttpGet("{userId}/player-advertisements/active")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetUserActivePlayerAdvertisements(string userId)
        {
            var userActivePlayerAdvertisements = await _userRepository.GetUserActivePlayerAdvertisements(userId);
            return Ok(userActivePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/inactive  ->  zwróć nieaktywne ogłoszenia dla konkretnego użytkownika
        [HttpGet("{userId}/player-advertisements/inactive")]
        public async Task<ActionResult<IEnumerable<PlayerAdvertisement>>> GetUserInactivePlayerAdvertisements(string userId)
        {
            var userInactivePlayerAdvertisements = await _userRepository.GetUserInactivePlayerAdvertisements(userId);
            return Ok(userInactivePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/favorites  ->  zwróć ulubione ogłoszenia dla konkretnego użytkownika
        [HttpGet("{userId}/player-advertisements/favorites")]
        public async Task<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>> GetUserFavoritePlayerAdvertisements(string userId)
        {
            var userFavoritePlayerAdvertisements = await _userRepository.GetUserFavoritePlayerAdvertisements(userId);
            return Ok(userFavoritePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/favorites/active  ->  zwróć aktywne ulubione ogłoszenia dla konkretnego użytkownika
        [HttpGet("{userId}/player-advertisements/favorites/active")]
        public async Task<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>> GetUserActiveFavoritePlayerAdvertisements(string userId)
        {
            var userActiveFavoritePlayerAdvertisements = await _userRepository.GetUserActiveFavoritePlayerAdvertisements(userId);
            return Ok(userActiveFavoritePlayerAdvertisements);
        }

        // GET: api/users/:userId/player-advertisements/favorites/inactive  ->  zwróć nieaktywne ulubione ogłoszenia dla konkretnego użytkownika
        [HttpGet("{userId}/player-advertisements/favorites/inactive")]
        public async Task<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>> GetUserInactiveFavoritePlayerAdvertisements(string userId)
        {
            var userInactiveFavoritePlayerAdvertisements = await _userRepository.GetUserInactiveFavoritePlayerAdvertisements(userId);
            return Ok(userInactiveFavoritePlayerAdvertisements);
        }

        // GET: api/users/:userId/club-offers/received  ->  zwróć otrzymane oferty klubowe dla konkretnego użytkownika
        [HttpGet("{userId}/club-offers/received")]
        public async Task<ActionResult<IEnumerable<ClubOffer>>> GetReceivedClubOffers(string userId)
        {
            var receivedClubOffers = await _userRepository.GetReceivedClubOffers(userId);
            return Ok(receivedClubOffers);
        }

        // GET: api/users/:userId/club-offers/sent  ->  zwróć wysłane ogłoszenia klubowe dla konkretnego użytkownika
        [HttpGet("{userId}/club-offers/sent")]
        public async Task<ActionResult<IEnumerable<ClubOffer>>> GetSentClubOffers(string userId)
        {
            var sentClubOffers = await _userRepository.GetSentClubOffers(userId);
            return Ok(sentClubOffers);
        }

        // GET: api/users/:userId/chats  ->  zwróć czaty dla konkretnego użytkownika
        [HttpGet("{userId}/chats")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetUserChats(string userId)
        {
            var userChats = await _userRepository.GetUserChats(userId);
            return Ok(userChats);
        }

        // GET: api/users/export  ->  eksportuj użytkowników do pliku .csv
        [HttpGet("export")]
        public async Task<IActionResult> ExportUsersToCsv()
        {
            var csvStream = await _userRepository.ExportUsersToCsv();
            return File(csvStream, "text/csv", "users.csv");
        }
    }
}