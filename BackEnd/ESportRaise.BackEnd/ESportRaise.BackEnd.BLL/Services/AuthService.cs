using ESportRaise.BackEnd.BLL.DTOs.Tokens;
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

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByEmailOrUserNameAsync(loginRequest.EmailOrUserName);
            if (user != null && usersRepository.IsUserPasswordCorrect(user, loginRequest.Password))
            {
                var tokenClaims = GetTokenClaimsForUser(user);
                var refreshToken = refreshTokenFactory.GenerateToken();
                await usersRepository.CreateRefreshTokenAsync(user, refreshToken);

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

        private IEnumerable<Claim> GetTokenClaimsForUser(AppUser user)
        {
            var userClaims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole)
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

            var user = new AppUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                UserRole = registerRequest.Role
            };
            await usersRepository.CreateAsync(user, registerRequest.Password);

            return registerResponse;
        }

        public async Task<TokenRefreshResponse> RefreshTokenAsync(TokenRefreshRequest refreshRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByUserNameAsync(refreshRequest.UserName);
            if (user == null)
            {
                return new TokenRefreshResponse("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshToken(user, refreshRequest.RefreshToken);
            if (!validRefreshToken)
            {
                return new TokenRefreshResponse("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, refreshRequest.RefreshToken);

            var newRefreshToken = refreshTokenFactory.GenerateToken();
            await usersRepository.CreateRefreshTokenAsync(user, newRefreshToken);

            var userClaims = GetTokenClaimsForUser(user);
            return new TokenRefreshResponse
            {
                Token = tokenFactory.GenerateTokenForClaims(userClaims),
                RefreshToken = newRefreshToken
            };
        }

        public async Task<TokenRevokeResponse> RevokeTokenAsync(TokenRevokeRequest revokeRequest)
        {
            var user = await usersRepository.GetUserOrDefaultByUserNameAsync(revokeRequest.UserName);
            if (user == null)
            {
                return new TokenRevokeResponse("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshToken(user, revokeRequest.RefreshToken);
            if (!validRefreshToken)
            {
                return new TokenRevokeResponse("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, revokeRequest.RefreshToken);

            return new TokenRevokeResponse();
        }
    }
}
