namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla achievmentsów w club history
    public class AchievementsDTO
    {
        public int NumberOfMatches { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public string AdditionalAchievements { get; set; }
    }
}