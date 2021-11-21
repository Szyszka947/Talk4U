using IdentityModel;
using PeerToPeerCall.Services.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace PeerToPeerCall.ServicesImpl.JWT
{
    public class GetCredentialsFromAccessTokenServiceImpl : IGetCredentialsFromAccessTokenService
    {
        public (int Id, string UserName) Get(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = tokenHandler.ReadJwtToken(accessToken);

            return (int.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == JwtClaimTypes.Id).Value),
                jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}
