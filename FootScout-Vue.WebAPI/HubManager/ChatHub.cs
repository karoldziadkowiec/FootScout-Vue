using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FootScout_Vue.WebAPI.HubManager
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(MessageSendDTO messageSendDTO)
        {
            var message = await _messageService.SendMessage(messageSendDTO);
            await Clients.Group(messageSendDTO.ChatId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}