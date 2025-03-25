using FootScout_Vue.WebAPI.Entities;

namespace FootScout_Vue.WebAPI.Services.Interfaces
{
    // Interfejs deklarujący operacje związane z czatem
    public interface IChatService
    {
        Task<Chat> GetChatById(int chatId);
        Task<IEnumerable<Chat>> GetChats();
        Task<int> GetChatCount();
        Task<int> GetChatIdBetweenUsers(string user1Id, string user2Id);
        Task CreateChat(Chat chat);
        Task DeleteChat(int chatId);
        Task<MemoryStream> ExportChatsToCsv();
    }
}