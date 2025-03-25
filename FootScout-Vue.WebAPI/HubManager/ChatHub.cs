using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FootScout_Vue.WebAPI.HubManager
{
    // Klasa wspomagająca komunikację w czasie rzeczywistym z wykorzystaniem SignalR
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // Wysyłanie wiadomości w czasie rzeczywistym + zapis do bazy danych
        public async Task SendMessage(MessageSendDTO messageSendDTO)
        {
            var message = await _messageService.SendMessage(messageSendDTO);
            await Clients.Group(messageSendDTO.ChatId.ToString()).SendAsync("ReceiveMessage", message);
        }

        // Dołącz do czatu
        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        // Opuść czat
        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}