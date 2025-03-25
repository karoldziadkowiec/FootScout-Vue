using System.ComponentModel.DataAnnotations;

namespace FootScout_Vue.WebAPI.Models.DTOs
{
    // Model DTO dla logowania
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}