using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeerToPeerCall.Services.JWT;
using System;
using System.Threading.Tasks;

namespace PeerToPeerCall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTController : ControllerBase
    {
        private readonly IValidateJWTService _validateJWTService;
        private readonly IGetCredentialsFromAccessTokenService _getCredentialsFromAccessTokenService;
        private readonly IGenerateJWTService _generateJWTService;

        public JWTController(IValidateJWTService validateJWTService, IGetCredentialsFromAccessTokenService getCredentialsFromAccessTokenService,
            IGenerateJWTService generateJWTService)
        {
            _validateJWTService = validateJWTService;
            _getCredentialsFromAccessTokenService = getCredentialsFromAccessTokenService;
            _generateJWTService = generateJWTService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> GetAccessTokenByRefreshToken()
        {
            (string refreshToken, string accessToken) = (HttpContext.Request.Cookies["refreshToken"], HttpContext.Request.Cookies["accessToken"]);

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken) || !_validateJWTService.IsExpiredAccessTokenValid(accessToken))
                return Unauthorized(false);

            var (Id, UserName) = _getCredentialsFromAccessTokenService.Get(accessToken);

            var isRefreshTokenValid = await _validateJWTService.IsRefreshTokenValidAsync(refreshToken, Id);

            if (isRefreshTokenValid)
                HttpContext.Response.Cookies.Append("accessToken", _generateJWTService.GenerateAccessToken(Id, UserName), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = false,
                    MaxAge = TimeSpan.FromDays(365),
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });

            return isRefreshTokenValid ? Ok(true) : Unauthorized(false);
        }

        [HttpGet("validate/access")]
        public bool ValidateAccessToken()
        {
            return HttpContext.User.Identity.IsAuthenticated;
        }
    }
}