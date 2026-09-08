using BackendESP.Data;
using BackendESP.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendESP.Services
{
    public class TokenService : ITokenService
    {
        private readonly IPlayerData _playerData;
        private readonly IConfiguration _config;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(IConfiguration config, IPlayerData playerData)
        {
            _config = config;
            _key = config["JwtSettings:Key"];
            _issuer = config["JwtSettings:Issuer"];
            _audience = config["JwtSettings:Audience"];
            _playerData = playerData;
        }

        public async Task<string> GenerateToken(PlayerDTO player)
        {
            var p = await _playerData.GetPlayerAsyncByEmailToken(player.Email);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, p.PlayerID.ToString()),
                new Claim(ClaimTypes.Email, player.Email),
                new Claim(ClaimTypes.Name, p.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            string t = new JwtSecurityTokenHandler().WriteToken(token);
            return t;
        }
    }
}
