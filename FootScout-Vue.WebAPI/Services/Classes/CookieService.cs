using FootScout_Vue.WebAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_Vue.WebAPI.Services.Classes
{
    // Serwis z zaimplementowanymi metodami związanymi z ciasteczkami
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Ustaw ciasteczka
        public async Task SetCookies(JwtSecurityToken token, string tokenString)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Using only HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = token.ValidTo
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("AuthToken", tokenString, cookieOptions);
            await Task.CompletedTask;
        }
    }
}