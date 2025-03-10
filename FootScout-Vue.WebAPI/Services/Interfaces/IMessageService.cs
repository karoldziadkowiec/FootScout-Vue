using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;

namespace FootScout_Vue.WebAPI.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessages();
        Task<int> GetAllMessagesCount();
        Task<IEnumerable<Message>> GetMessagesForChat(int chatId);
        Task<int> GetMessagesForChatCount(int chatId);
        Task<DateTime> GetLastMessageDateForChat(int chatId);
        Task<Message> SendMessage(MessageSendDTO dto);
        Task DeleteMessage(int messageId);
    }
}