using ESportRaise.BackEnd.BLL.DTOs;
using ESportRaise.BackEnd.BLL.DTOs.Auth;
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
        private UserAsyncRepository usersRepository;

        private IAuthTokenFactory tokenFactory;

        private IRefreshTokenFactory refreshTokenFactory;

        public AuthService(UserAsyncRepository users, IAuthTokenFactory tokenFactory, IRefreshTokenFactory refreshTokenFactory)
        {
            this.usersRepository = users;
            this.tokenFactory = tokenFactory;
            this.refreshTokenFactory = refreshTokenFactory;
        }

        public async Task<LoginServiceResponse> LoginAsync(LoginServiceRequest loginRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByEmailOrUserNameAsync(loginRequest.EmailOrUserName);
            if (user != null && usersRepository.IsUserPasswordCorrect(user, loginRequest.Password))
            {
                var tokenClaims = GetTokenClaimsForUser(user);
                var refreshToken = refreshTokenFactory.GenerateToken();
                await usersRepository.CreateRefreshTokenAsync(user, refreshToken);

                return new LoginServiceResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = tokenFactory.GenerateTokenForClaims(tokenClaims),
                    RefreshToken = refreshToken
                };
            }
            return new LoginServiceResponse("Email, user name or password is incorrect!");
        }

        private IEnumerable<Claim> GetTokenClaimsForUser(AppUser user)
        {
            var userClaims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole)
            };
            return userClaims;
        }

        public async Task<RegisterServiceResponse> RegisterAsync(RegisterServiceRequest registerRequest)
        {
            var registerResponse = new RegisterServiceResponse();

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

            var user = new AppUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                UserRole = registerRequest.Role
            };
            await usersRepository.CreateAsync(user, registerRequest.Password);

            return registerResponse;
        }

        public async Task<TokenServiceRefreshResponse> RefreshTokenAsync(TokenServiceRefreshRequest refreshRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByUserNameAsync(refreshRequest.UserName);
            if (user == null)
            {
                return new TokenServiceRefreshResponse("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshToken(user, refreshRequest.RefreshToken);
            if (!validRefreshToken)
            {
                return new TokenServiceRefreshResponse("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, refreshRequest.RefreshToken);

            var newRefreshToken = refreshTokenFactory.GenerateToken();
            await usersRepository.CreateRefreshTokenAsync(user, newRefreshToken);

            var userClaims = GetTokenClaimsForUser(user);
            return new TokenServiceRefreshResponse
            {
                Token = tokenFactory.GenerateTokenForClaims(userClaims),
                RefreshToken = newRefreshToken
            };
        }

        public async Task<TokenServiceRevokeResponse> RevokeTokenAsync(TokenServiceRevokeRequest revokeRequest)
        {
            var user = await usersRepository.GetUserOrDefaultByUserNameAsync(revokeRequest.UserName);
            if (user == null)
            {
                return new TokenServiceRevokeResponse("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshToken(user, revokeRequest.RefreshToken);
            if (!validRefreshToken)
            {
                return new TokenServiceRevokeResponse("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, revokeRequest.RefreshToken);

            return new TokenServiceRevokeResponse();
        }
    }
}
