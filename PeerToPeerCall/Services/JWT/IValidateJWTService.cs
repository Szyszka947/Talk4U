using System.Threading.Tasks;

namespace PeerToPeerCall.Services.JWT
{
    public interface IValidateJWTService
    {
        Task<bool> IsRefreshTokenValidAsync(string refreshToken, int userId);
        bool IsExpiredAccessTokenValid(string accessToken);
    }
}
