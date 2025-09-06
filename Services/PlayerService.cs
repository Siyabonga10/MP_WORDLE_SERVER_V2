using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MP_WORDLE_SERVER_V2.Data;
using System.Text;
using MP_WORDLE_SERVER_V2.Models;

using Microsoft.AspNetCore.Identity;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class PlayerService
    {
        private readonly GameDb _DbContext;
        private readonly PasswordHasher<Player> _Hasher;
        public PlayerService(GameDb DbContext)
        {
            _DbContext = DbContext;
            _Hasher = new();
        }
        public void AddJWTToPlayer(Player player, HttpResponse response) // For testing purposes, I'll just assume that this is where the player is also being created, no validation yet !!!
        {
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
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds);

            response.Cookies.Append("jwt_token", new JwtSecurityTokenHandler().WriteToken(token), new CookieOptions
            {
                HttpOnly = true,  // Cannot be accessed by JavaScript
                Secure = false,    
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });
        }

        public async Task<(Player? NewPlayer, string OutcomeMsg)> CreatePlayer(string username, string password)
        {
            var usernameExists = await _DbContext.Players.AnyAsync(player => player.Username == username);
            if (usernameExists)
                return (null, "Username already exists");

            Player newPlayer = new(Guid.NewGuid(), username);
            newPlayer.Password = _Hasher.HashPassword(newPlayer, password);
            await _DbContext.Players.AddAsync(newPlayer);
            await _DbContext.SaveChangesAsync();

            return (newPlayer, "User created");
        }

        public async Task<Player?> GetPlayerFromCredentials(string username, string password)
        {
            var target_user = await _DbContext.Players.FirstOrDefaultAsync(player => player.Username == username);
            if (target_user == null)
                return null;

            if (_Hasher.VerifyHashedPassword(target_user, target_user.Password, password) == PasswordVerificationResult.Success)
                return target_user;

            return null;
        }
    }
}