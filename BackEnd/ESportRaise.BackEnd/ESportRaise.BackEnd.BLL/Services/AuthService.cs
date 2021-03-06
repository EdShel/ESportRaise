﻿using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.DTOs.Auth;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class AuthService : IAuthAsyncService
    {
        private AppUserRepository usersRepository;

        private IAuthTokenFactory tokenFactory;

        private IRefreshTokenFactory refreshTokenFactory;

        public AuthService(AppUserRepository users, IAuthTokenFactory tokenFactory, IRefreshTokenFactory refreshTokenFactory)
        {
            this.usersRepository = users;
            this.tokenFactory = tokenFactory;
            this.refreshTokenFactory = refreshTokenFactory;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByEmailOrUserNameAsync(loginRequest.EmailOrUserName);
            if (user != null && usersRepository.IsUserPasswordCorrect(user, loginRequest.Password))
            {
                var tokenClaims = GetTokenClaimsForUser(user);
                var refreshToken = refreshTokenFactory.GenerateToken();
                await usersRepository.CreateRefreshTokenAsync(user, refreshToken);

                return new LoginResponseDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = tokenFactory.GenerateTokenForClaims(tokenClaims),
                    RefreshToken = refreshToken
                };
            }
            throw new BadRequestException("Email, user name or password is incorrect!");
        }

        private IEnumerable<Claim> GetTokenClaimsForUser(AppUser user)
        {
            var userClaims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole),
                new Claim(AuthConstants.ID_CLAIM_TYPE, user.Id.ToString())
            };
            return userClaims;
        }

        public async Task RegisterAsync(RegisterDTO registerRequest)
        {
            bool emailIsTaken = await usersRepository.IsAnyUserWithEmailAsync(registerRequest.Email);
            if (emailIsTaken)
            {
                throw new BadRequestException("Email is taken!");
            }

            bool nameIsTaken = await usersRepository.IsAnyUserWithUserNameAsync(registerRequest.UserName);
            if (nameIsTaken)
            {
                throw new BadRequestException("User name is taken!");
            }

            var user = new AppUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                UserRole = registerRequest.Role
            };
            await usersRepository.CreateAsync(user, registerRequest.Password);
        }

        public async Task<TokenRefreshResponseDTO> RefreshTokenAsync(TokenRefreshRequestDTO refreshRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByUserNameAsync(refreshRequest.UserName);
            if (user == null)
            {
                throw new BadRequestException("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshTokenAsync(user, refreshRequest.RefreshToken);
            if (!validRefreshToken)
            {
                throw new BadRequestException("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, refreshRequest.RefreshToken);

            var newRefreshToken = refreshTokenFactory.GenerateToken();
            await usersRepository.CreateRefreshTokenAsync(user, newRefreshToken);

            var userClaims = GetTokenClaimsForUser(user);
            return new TokenRefreshResponseDTO
            {
                Token = tokenFactory.GenerateTokenForClaims(userClaims),
                RefreshToken = newRefreshToken
            };
        }

        public async Task RevokeTokenAsync(TokenRevokeDTO revokeRequest)
        {
            var user = await usersRepository.GetUserOrDefaultByUserNameAsync(revokeRequest.UserName);
            if (user == null)
            {
                throw new BadRequestException("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshTokenAsync(user, revokeRequest.RefreshToken);
            if (!validRefreshToken)
            {
                throw new BadRequestException("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, revokeRequest.RefreshToken);
        }
    }
}
