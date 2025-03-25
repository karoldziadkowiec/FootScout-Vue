namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla resetowania hasła użytkownika
    public class UserResetPasswordDTO
    {
        public string PasswordHash { get; set; }
        public string ConfirmPasswordHash { get; set; }
    }
}