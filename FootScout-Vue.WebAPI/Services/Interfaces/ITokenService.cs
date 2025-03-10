using FootScout_Vue.WebAPI.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_Vue.WebAPI.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateTokenJWT(User user);
    }
}