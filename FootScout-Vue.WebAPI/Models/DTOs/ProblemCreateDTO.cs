namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla tworzenia problemu aplikacji
    public class ProblemCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequesterId { get; set; }
    }
}