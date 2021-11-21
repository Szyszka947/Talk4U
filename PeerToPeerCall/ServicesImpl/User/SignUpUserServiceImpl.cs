using Microsoft.AspNetCore.Http;
using PeerToPeerCall.Data.Entities;
using PeerToPeerCall.Interfaces.User;
using PeerToPeerCall.Models;
using PeerToPeerCall.Services.JWT;
using System.Collections.Generic;

namespace PeerToPeerCall.Services.User
{
    public class SignUpUserServiceImpl : ISignUpUserService
    {
        private readonly ICreateUserService _createUserService;
        private readonly IUniqueUserCredentialsAvailableService _uniqueUserCredentialsAvailableService;
        private readonly IGenerateJWTService _generateJWTService;
        private readonly IAssignRefreshTokenService _assignRefreshTokenService;

        public SignUpUserServiceImpl(ICreateUserService createUserService, IUniqueUserCredentialsAvailableService userCredentialsAvailableService,
            IGenerateJWTService generateJWTService, IAssignRefreshTokenService assignRefreshTokenService)
        {
            _createUserService = createUserService;
            _uniqueUserCredentialsAvailableService = userCredentialsAvailableService;
            _generateJWTService = generateJWTService;
            _assignRefreshTokenService = assignRefreshTokenService;
        }

        public ApiResponse SignUpUser(UserSignUpDto userSignUpDto)
        {
            var data = new Dictionary<string, List<string>>();

            if (!_uniqueUserCredentialsAvailableService.IsEmailAvailable(userSignUpDto.Email)) data.Add(nameof(UserEntity.Email), new() { "Email is taken" });
            if (!_uniqueUserCredentialsAvailableService.IsUserNameAvailable(userSignUpDto.UserName)) data.Add(nameof(UserEntity.UserName), new() { "Username is taken" });

            if (data.Keys.Count > 0)
            {
                return new ApiResponse
                {
                    Status = ResponseTypes.Fail,
                    Message = "One or more unique user credentials are already in use",
                    Code = StatusCodes.Status409Conflict,
                    Data = data
                };
            }

            var newUser = new UserEntity
            {
                Email = userSignUpDto.Email,
                UserName = userSignUpDto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userSignUpDto.Password),
            };

            _createUserService.CreateUser(newUser);

            var newRefreshToken = _generateJWTService.GenerateRefreshToken(newUser);

            _assignRefreshTokenService.AssignRefreshToken(_generateJWTService.GenerateRefreshToken(newUser));

            return new ApiResponse
            {
                Status = ResponseTypes.Success,
                Message = "Signing up completed successfully",
                Code = StatusCodes.Status201Created,
                Data = { { "refreshToken", new() { newRefreshToken.RefreshToken } } }
            };
        }
    }
}
