using ESportRaise.BackEnd.BLL.DTOs.Tokens;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Repositories;
using System;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class AuthService : IAuthService
    {
        private UserAsyncRepository usersRepository;

        private ITokenFactory tokenFactory;

        public AuthService(UserAsyncRepository users, ITokenFactory tokenFactory)
        {
            this.usersRepository = users;
            this.tokenFactory = tokenFactory;
        }

        public LoginResponse Login(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }

        public RegisterResponse Register(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }

        public TokenRefreshResponse RefreshToken(TokenRefreshRequest refreshRequest)
        {
            throw new NotImplementedException();
        }

        public TokenRevokeResponse RevokeToken(TokenRevokeRequest revokeRequest)
        {
            throw new NotImplementedException();
        }
    }
}
