using PeerToPeerCall.Data.Entities;
using System.Threading.Tasks;

namespace PeerToPeerCall.Services.JWT
{
    public interface ISearchRefreshTokenService
    {
        Task<RefreshTokenEntity> SearchRefreshTokenByTokenAsync(string refreshToken);
        Task<RefreshTokenEntity> SearchRefreshTokenByUserIdAsync(int userId);
    }
}
