using Microsoft.IdentityModel.Tokens;
using PeerToPeerCall.Data;
using PeerToPeerCall.Services.JWT;
using PeerToPeerCall.Services.User;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace PeerToPeerCall.ServicesImpl.JWT
{
    public class ValidateJWTServiceImpl : IValidateJWTService
    {
        private readonly ISearchRefreshTokenService _searchRefreshTokenService;

        public ValidateJWTServiceImpl(ISearchRefreshTokenService searchRefreshTokenService)
        {
            _searchRefreshTokenService = searchRefreshTokenService;
        }

        public bool IsExpiredAccessTokenValid(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(accessToken)) return false;
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidAudience = JWTCfg.ValidAudience,
                    ValidIssuer = JWTCfg.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWTCfg.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                }, out _);

                return claimsPrincipal.Identity.IsAuthenticated;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken, int userId)
        {
            var token = await _searchRefreshTokenService.SearchRefreshTokenByTokenAsync(refreshToken);

            if (token == default) return false;

            if (!token.IsExpired && token.User.Id == userId) return true;

            return false;
        }
    }
}