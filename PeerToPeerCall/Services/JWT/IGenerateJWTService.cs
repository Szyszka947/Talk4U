using PeerToPeerCall.Data.Entities;

namespace PeerToPeerCall.Services.JWT
{
    public interface IGenerateJWTService
    {
        string GenerateAccessToken(int userId, string userName);
        RefreshTokenEntity GenerateRefreshToken(UserEntity user);
    }
}
