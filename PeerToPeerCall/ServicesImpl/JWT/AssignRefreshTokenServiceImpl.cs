using PeerToPeerCall.Data.AppDbContext;
using PeerToPeerCall.Data.Entities;
using PeerToPeerCall.Services.JWT;
using System.Linq;

namespace PeerToPeerCall.ServicesImpl.JWT
{
    public class AssignRefreshTokenServiceImpl : IAssignRefreshTokenService
    {
        private readonly AppDbContext _dbContext;
        private readonly IGenerateJWTService _generateJWTService;


        public AssignRefreshTokenServiceImpl(AppDbContext dbContext, IGenerateJWTService generateJWTService)
        {
            _dbContext = dbContext;
            _generateJWTService = generateJWTService;
        }

        public void AssignRefreshToken(RefreshTokenEntity refreshToken)
        {
            _dbContext.RefreshTokens.Add(refreshToken);
            _dbContext.SaveChanges();
        }

        public RefreshTokenEntity ReplaceRefreshToken(RefreshTokenEntity oldRefreshToken)
        {
            var newRefreshToken = _generateJWTService.GenerateRefreshToken(oldRefreshToken.User);

            _dbContext.RefreshTokens.Where(p => p == oldRefreshToken).ToList().ForEach(p =>
            {
                p.RefreshToken = newRefreshToken.RefreshToken;
                p.ExpiresAt = newRefreshToken.ExpiresAt;
            });

            _dbContext.SaveChanges();

            return newRefreshToken;
        }
    }
}
