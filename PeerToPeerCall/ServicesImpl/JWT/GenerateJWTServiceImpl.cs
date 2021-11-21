using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using PeerToPeerCall.Data;
using PeerToPeerCall.Data.Entities;
using PeerToPeerCall.Services.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeerToPeerCall.ServicesImpl.JWT
{
    public class GenerateJWTServiceImpl : IGenerateJWTService
    {
        public string GenerateAccessToken(int userId, string userName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTCfg.SecretKey));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var securityToken = new JwtSecurityToken(
                issuer: JWTCfg.ValidIssuer,
                audience: JWTCfg.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: signingCredentials,
                claims: new List<Claim>
                {
                    new Claim(JwtClaimTypes.Id, userId.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userName)
                });

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        public RefreshTokenEntity GenerateRefreshToken(UserEntity user)
        {
            return new RefreshTokenEntity
            {
                User = user,
                RefreshToken = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(365),
            };
        }
    }
}
