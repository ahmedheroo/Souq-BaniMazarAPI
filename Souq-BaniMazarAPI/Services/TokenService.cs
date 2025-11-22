using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Souq_BaniMazarAPI.Data;
using Souq_BaniMazarAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Souq_BaniMazarAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _config = config;
            _userManager = userManager;
            _db = db;
        }
        public async Task<(string accessToken, string refreshToken)> CreateTokensAsync(ApplicationUser user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("name", user.Name ?? "")
        }.Union(userRoles.Select(r => new Claim(ClaimTypes.Role, r))).ToList();

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["AccessTokenExpirationMinutes"] ?? "15")),
                signingCredentials: creds
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSection["RefreshTokenExpirationDays"] ?? "30")),
                IsRevoked = false
            };
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return (accessToken, refreshToken.Token);
        }
        public async Task<(string accessToken, string refreshToken)?> RefreshAsync(string refreshToken)
        {
            var existing = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (existing == null || existing.IsRevoked || existing.Expires < DateTime.UtcNow) return null;

            var user = await _userManager.FindByIdAsync(existing.UserId);
            if (user == null) return null;

            existing.IsRevoked = true;
            existing.ReplacedByToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var newRefresh = new RefreshToken
            {
                Token = existing.ReplacedByToken!,
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? "30")),
                IsRevoked = false
            };
            _db.RefreshTokens.Add(newRefresh);
            await _db.SaveChangesAsync();

            var (access, _) = await CreateTokensAsync(user);  

            return (access, newRefresh.Token);
        }
        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var existing = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (existing == null) return;
            existing.IsRevoked = true;
            await _db.SaveChangesAsync();
        }
        public async Task<string?> GetUserIdFromRefreshTokenAsync(string refreshToken)
        {
            var existing = await _db.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            return existing?.UserId;
        }
    }
}
