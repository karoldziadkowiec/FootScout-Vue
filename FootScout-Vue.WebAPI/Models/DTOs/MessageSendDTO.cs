namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla tworzenia nowej wiadomości w czacie
    public class MessageSendDTO
    {
        public int ChatId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}