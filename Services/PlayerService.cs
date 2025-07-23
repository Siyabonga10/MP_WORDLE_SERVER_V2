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
        private readonly GameDb _DbContext;
        public PlayerService(GameDb DbContext)
        {
            _DbContext = DbContext;
        }
        public async Task<string> GenerateJwtTokenAsync(Player player) // For testing purposes, I'll just assume that this is where the player is also being created, no validation yet !!!
        {
            player.Id = Guid.NewGuid();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, player.Username),
                new Claim(JwtRegisteredClaimNames.Jti, player.Id.ToString())
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

            await _DbContext.Players.AddAsync(player);
            await _DbContext.SaveChangesAsync();

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}