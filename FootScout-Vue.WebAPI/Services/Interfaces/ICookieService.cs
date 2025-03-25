using System.IdentityModel.Tokens.Jwt;

namespace FootScout_Vue.WebAPI.Services.Interfaces
{
    // Interfejs deklarujący operacje związane z obsługą ciasteczek
    public interface ICookieService
    {
        Task SetCookies(JwtSecurityToken token, string tokenString);
    }
}