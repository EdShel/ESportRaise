﻿using ESportRaise.BackEnd.BLL.DTOs.Tokens;
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

        public AuthService(UserAsyncRepository users, ITokenFactory tokenFactory)
        {
            this.usersRepository = users;
            this.tokenFactory = tokenFactory;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            User user = await usersRepository.GetUserOrDefaultByEmailOrUserNameAsync(loginRequest.EmailOrUserName);
            if (user != null && usersRepository.IsUserPasswordCorrect(user, loginRequest.Password))
            {
                var tokenClaims = GetTokenClaimsForUser(user);
                return new LoginResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = tokenFactory.GenerateTokenForClaims(tokenClaims),
                    RefreshToken = ""
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

        public async Task<TokenRefreshResponse> RefreshTokenAsync(TokenRefreshRequest refreshRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenRevokeResponse> RevokeTokenAsync(TokenRevokeRequest revokeRequest)
        {
            throw new NotImplementedException();
        }
    }
}
