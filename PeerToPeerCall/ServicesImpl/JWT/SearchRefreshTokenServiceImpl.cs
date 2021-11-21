using Microsoft.EntityFrameworkCore;
using PeerToPeerCall.Data.AppDbContext;
using PeerToPeerCall.Data.Entities;
using PeerToPeerCall.Services.JWT;
using System.Threading.Tasks;

namespace PeerToPeerCall.ServicesImpl.JWT
{
    public class SearchRefreshTokenServiceImpl : ISearchRefreshTokenService
    {
        private readonly AppDbContext _dbContext;

        public SearchRefreshTokenServiceImpl(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<RefreshTokenEntity> SearchRefreshTokenByTokenAsync(string refreshToken) =>
            await _dbContext.RefreshTokens.Include(p => p.User).SingleOrDefaultAsync(p => p.RefreshToken == refreshToken);

        public async Task<RefreshTokenEntity> SearchRefreshTokenByUserIdAsync(int userId) =>
            await _dbContext.RefreshTokens.Include(p => p.User).SingleOrDefaultAsync(p => p.User.Id == userId);
    }
}
