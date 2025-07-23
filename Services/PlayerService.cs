using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MP_WORDLE_SERVER_V2.Data;
using System.Text;
using MP_WORDLE_SERVER_V2.Models;
using Microsoft.Identity.Client;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class PlayerService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public PlayerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public string GenerateJwtToken(Player player)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, player.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwt_key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // Just doing a's since the key need to be a minimum of 128 bits
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MpWordle.com",
                audience: "MpWordle.com",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}