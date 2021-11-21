using PeerToPeerCall.Data.Entities;

namespace PeerToPeerCall.Services.JWT
{
    public interface IAssignRefreshTokenService
    {
        void AssignRefreshToken(RefreshTokenEntity refreshToken);
        RefreshTokenEntity ReplaceRefreshToken(RefreshTokenEntity oldRefreshToken);
    }
}
