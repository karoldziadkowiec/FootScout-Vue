namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla aktualizacji profilu użytkownika
    public class UserUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
    }
}
