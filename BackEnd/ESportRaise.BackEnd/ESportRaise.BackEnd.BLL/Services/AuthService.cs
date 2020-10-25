using ESportRaise.BackEnd.BLL.DTOs.Tokens;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using System;
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
            if (user )
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
