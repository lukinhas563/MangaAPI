using MangaAPI.Entities;
using MangaAPI.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MangaAPI.Services
{
    public class TokenService
    {
        private readonly TokenSettings _settings;
        public TokenService(TokenSettings settings)
        {
            _settings = settings;
        }

        public string Generate(User user)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            byte[] key = Encoding.ASCII.GetBytes(_settings.PrivateKey);
            SigningCredentials credencials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(user),
                SigningCredentials = credencials,
                Expires = DateTime.UtcNow.AddHours(8),
            };

            var token = handler.CreateToken(tokenDescriptor);

            string stringToken = handler.WriteToken(token);

            return stringToken;
        }

        private static ClaimsIdentity GenerateClaims(User user)
        {
            ClaimsIdentity claim = new ClaimsIdentity();

            claim.AddClaim(new Claim("id", user.Id.ToString()));
            claim.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            claim.AddClaim(new Claim("last_name", user.LastName));
            claim.AddClaim(new Claim("username", user.Username));
            claim.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claim.AddClaim(new Claim(ClaimTypes.Role, user.UserType.ToString()));

            return claim;
        }
    }
}
