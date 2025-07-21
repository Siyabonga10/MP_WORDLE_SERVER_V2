using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MP_WORDLE_SERVER_V2.Data;
using System.Text;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class PlayerService
    {
        readonly private GameDb _DbContext;
        private int AvailableId = 0;
        public PlayerService(GameDb dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task InitPlayerService()
        {
            var lastPlayer = await _DbContext.Players
                .OrderByDescending(player => player.Id)
                .FirstAsync();
            if (lastPlayer == null) return;
            AvailableId = lastPlayer.Id;
        }

        private int GetNewId()
        {
            return AvailableId++;
        }

        public string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, GetNewId().ToString())
            };

            var jwt_key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "no_key";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MpWordle.com",
                audience: "MpWordle.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}