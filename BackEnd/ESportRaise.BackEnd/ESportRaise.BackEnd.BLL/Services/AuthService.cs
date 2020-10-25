using ESportRaise.BackEnd.BLL.DTOs.Tokens;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class AuthService : IAuthAsyncService
    {
        private UserAsyncRepository usersRepository;

        private ITokenFactory tokenFactory;

        private IRefreshTokenFactory refreshTokenFactory;

        public AuthService(UserAsyncRepository users, ITokenFactory tokenFactory, IRefreshTokenFactory refreshTokenFactory)
        {
            this.usersRepository = users;
            this.tokenFactory = tokenFactory;
            this.refreshTokenFactory = refreshTokenFactory;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            User user = await usersRepository.GetUserOrDefaultByEmailOrUserNameAsync(loginRequest.EmailOrUserName);
            if (user != null && usersRepository.IsUserPasswordCorrect(user, loginRequest.Password))
            {
                var tokenClaims = GetTokenClaimsForUser(user);
                var refreshToken = refreshTokenFactory.GenerateToken();
                usersRepository.CreateRefreshTokenAsync(user, refreshToken);

                return new LoginResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = tokenFactory.GenerateTokenForClaims(tokenClaims),
                    RefreshToken = refreshToken
                };
            }
            return new LoginResponse
            (
                errors: new[]
                {
                    new OperationError("Email, user name or password is incorrect!")
                }
            );
        }

        private IEnumerable<Claim> GetTokenClaimsForUser(User user)
        {
            var userClaims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            return userClaims;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var registerResponse = new RegisterResponse();

            bool emailIsTaken = await usersRepository.IsAnyUserWithEmailAsync(registerRequest.Email);
            if (emailIsTaken)
            {
                registerResponse.AddError("Email is taken!");
            }

            bool nameIsTaken = await usersRepository.IsAnyUserWithUserNameAsync(registerRequest.UserName);
            if (nameIsTaken)
            {
                registerResponse.AddError("User name is taken!");
            }

            if (!registerResponse.Success)
            {
                return registerResponse;
            }

            var user = new User
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                Role = registerRequest.Role
            };
            await usersRepository.CreateAsync(user, registerRequest.Password);

            return registerResponse;
        }

        public async Task<TokenRefreshResponse> RefreshTokenAsync(TokenRefreshRequest refreshRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenRevokeResponse> RevokeTokenAsync(TokenRevokeRequest revokeRequest)
        {
            throw new NotImplementedException();
        }
    }
}
