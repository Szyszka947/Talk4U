using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeerToPeerCall.Interfaces.User;
using PeerToPeerCall.Models;
using PeerToPeerCall.Services.User;
using System;
using System.Threading.Tasks;

namespace PeerToPeerCall.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISignUpUserService _signUpUserService;
        private readonly ISignInUserService _signInUserService;


        public UserController(ISignUpUserService signUpUserService, ISignInUserService signInUserService)
        {
            _signUpUserService = signUpUserService;
            _signInUserService = signInUserService;
        }

        //POST api/user/signup
        [HttpPost("signup")]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp([FromBody] UserSignUpDto userSignUpDto)
        {
            var result = _signUpUserService.SignUpUser(userSignUpDto);

            if (result.Status == ResponseTypes.Success)
                HttpContext.Response.Cookies.Append("refreshToken", result.Data["refreshToken"][0], new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = false,
                    MaxAge = TimeSpan.FromDays(365),
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/jwt/refresh"
                });

            return StatusCode(result.Code, result);
        }

        //POST api/user/signin
        [HttpPost("signin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([FromBody] UserSignInDto userSignInDto)
        {

            var result = await _signInUserService.SignInUserAsync(userSignInDto);

            if (result.Status == ResponseTypes.Success)
            {
                HttpContext.Response.Cookies.Append("accessToken", result.Data["accessToken"][0], new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = false,
                    MaxAge = TimeSpan.FromDays(365),
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });
                HttpContext.Response.Cookies.Append("refreshToken", result.Data["refreshToken"][0], new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = false,
                    MaxAge = TimeSpan.FromDays(365),
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/jwt/refresh"
                });
            }

            return StatusCode(result.Code, result);
        }
    }
}
