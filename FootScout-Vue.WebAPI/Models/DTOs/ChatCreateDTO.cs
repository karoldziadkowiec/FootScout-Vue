namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla tworzenia nowego chat roomu
    public class ChatCreateDTO
    {
        public string User1Id { get; set; }
        public string User2Id { get; set; }
    }
}