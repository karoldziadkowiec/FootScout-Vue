using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FootScout_Vue.WebAPI.Services.Classes
{
    // Serwis z zaimplementowanymi metodami związanymi z tokenem JWT
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        // Tworzenie tokena JWT dla użytkownika i zwrócenia go w odpowiedzi
        public async Task<JwtSecurityToken> CreateTokenJWT(User user)
        {
            // ustaw claimsy bazując na danych użytkownika
            var userClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles is not null && userRoles.Any())
            {
                userClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            }

            // utwórz token JWT
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var expireDays = _configuration.GetValue<int>("JWT:ExpireDays");
            var credentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(expireDays),
                claims: userClaims,
                signingCredentials: credentials
                );

            return token;
        }
    }
}