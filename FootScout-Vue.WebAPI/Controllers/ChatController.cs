using AutoMapper;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    // Kontroler API dla czatu
    [Route("api/chats")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;

        public ChatController(IChatService chatService, IMapper mapper)
        {
            _chatService = chatService;
            _mapper = mapper;
        }

        // GET: api/chats/:chatId  ->  zwróć czat dla konkretnego id
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatById(int chatId)
        {
            var chat = await _chatService.GetChatById(chatId);
            if (chat == null)
                return NotFound($"Chat with ID {chatId} not found.");

            return Ok(chat);
        }

        // GET: api/chats  ->  zwróć wszystkie czaty
        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var chats = await _chatService.GetChats();
            return Ok(chats);
        }

        // GET: api/chats/count  ->  zwróć liczbę czatów w aplikacji
        [HttpGet("count")]
        public async Task<IActionResult> GetChatCount()
        {
            int count = await _chatService.GetChatCount();
            return Ok(count);
        }

        // GET: api/chats/between/:user1Id/:user2Id  ->  zwróć id czatu dla 2 użytkowników
        [HttpGet("between/{user1Id}/{user2Id}")]
        public async Task<IActionResult> GetChatIdBetweenUsers(string user1Id, string user2Id)
        {
            var chatId = await _chatService.GetChatIdBetweenUsers(user1Id, user2Id);
            return Ok(chatId);
        }

        // POST: api/chats  ->  utwórz nowy czat
        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] ChatCreateDTO dto)
        {
            var chat = _mapper.Map<Chat>(dto);
            await _chatService.CreateChat(chat);
            return CreatedAtAction(nameof(GetChatById), new { chatId = chat.Id }, chat);
        }

        // DELETE: api/chats/:chatId  ->  usuń konkretny czat
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            var chat = await _chatService.GetChatById(chatId);
            if (chat == null)
                return NotFound($"Chat with ID {chatId} not found.");

            await _chatService.DeleteChat(chatId);
            return NoContent();
        }

        // GET: api/chats/export  ->  eksportuj czaty do pliku .csv
        [HttpGet("export")]
        public async Task<IActionResult> ExportChatsToCsv()
        {
            var csvStream = await _chatService.ExportChatsToCsv();
            return File(csvStream, "text/csv", "chats.csv");
        }
    }
}