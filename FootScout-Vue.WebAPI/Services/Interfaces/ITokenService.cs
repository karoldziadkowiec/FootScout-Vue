using FootScout_Vue.WebAPI.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_Vue.WebAPI.Services.Interfaces
{
    // Interfejs deklarujący operacje związane z tokenami JWT
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateTokenJWT(User user);
    }
}