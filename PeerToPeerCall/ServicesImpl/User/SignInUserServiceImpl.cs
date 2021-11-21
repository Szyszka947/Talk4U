using Microsoft.AspNetCore.Http;
using PeerToPeerCall.Data.Entities;
using PeerToPeerCall.Models;
using PeerToPeerCall.Services.JWT;
using PeerToPeerCall.Services.User;
using System;
using System.Threading.Tasks;

namespace PeerToPeerCall.ServicesImpl.User
{
    public class SignInUserServiceImpl : ISignInUserService
    {
        private readonly ISearchUserByUniqueCredentialsService _searchUserByUniqueCredentialsService;
        private readonly IGenerateJWTService _generateJWTService;
        private readonly ISearchRefreshTokenService _searchRefreshTokenService;
        private readonly IValidateJWTService _validateJWTService;
        private readonly IAssignRefreshTokenService _assignRefreshTokenService;


        public SignInUserServiceImpl(ISearchUserByUniqueCredentialsService searchUserByUniqueCredentialsService, IGenerateJWTService generateJWTService,
            ISearchRefreshTokenService searchRefreshTokenService, IValidateJWTService validateJWTService, IAssignRefreshTokenService assignRefreshTokenService)
        {
            _searchUserByUniqueCredentialsService = searchUserByUniqueCredentialsService;
            _generateJWTService = generateJWTService;
            _searchRefreshTokenService = searchRefreshTokenService;
            _validateJWTService = validateJWTService;
            _assignRefreshTokenService = assignRefreshTokenService;
        }


        public async Task<ApiResponse> SignInUserAsync(UserSignInDto userSignInDto)
        {
            var user = await _searchUserByUniqueCredentialsService.SearchByUserNameAsync(userSignInDto.UserName);

            if (user == default)
                return new ApiResponse
                {
                    Status = ResponseTypes.Fail,
                    Message = "User not found",
                    Code = StatusCodes.Status404NotFound,
                    Data = { { nameof(UserEntity.UserName), new() { "No user found with this username" } } }
                };

            if (BCrypt.Net.BCrypt.EnhancedVerify(userSignInDto.Password ?? "", user.PasswordHash))
            {
                var refreshToken = await _searchRefreshTokenService.SearchRefreshTokenByUserIdAsync(user.Id);
                var isRefreshTokenValid = await _validateJWTService.IsRefreshTokenValidAsync(refreshToken.RefreshToken, user.Id);

                var newRefreshToken = isRefreshTokenValid ? refreshToken.RefreshToken : _assignRefreshTokenService.ReplaceRefreshToken(refreshToken).RefreshToken;

                return new ApiResponse
                {
                    Status = ResponseTypes.Success,
                    Message = "Signing in completed successfully",
                    Code = StatusCodes.Status200OK,
                    Data =
                    {
                        { "accessToken", new() { _generateJWTService.GenerateAccessToken(user.Id, userSignInDto.UserName) } },
                        { "refreshToken", new() { newRefreshToken } }
                    }
                };
            }
            else
                return new ApiResponse
                {
                    Status = ResponseTypes.Fail,
                    Message = "Invalid credentials",
                    Code = StatusCodes.Status401Unauthorized,
                    Data = { { nameof(userSignInDto.Password), new() { "Invalid password" } } }
                };
        }
    }
}
