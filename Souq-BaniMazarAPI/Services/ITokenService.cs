using Souq_BaniMazarAPI.Models;

namespace Souq_BaniMazarAPI.Services
{
    public interface ITokenService
    {
        Task<(string accessToken, string refreshToken)> CreateTokensAsync(ApplicationUser user);
        Task<(string accessToken, string refreshToken)?> RefreshAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task<string?> GetUserIdFromRefreshTokenAsync(string refreshToken);
    }
}
