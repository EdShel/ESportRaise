using ESportRaise.BackEnd.BLL.DTOs.Tokens;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IAuthService
    {
        LoginResponse Login(LoginRequest loginRequest);

        RegisterResponse Register(RegisterRequest registerRequest);

        TokenRefreshResponse RefreshToken(TokenRefreshRequest refreshRequest);

        TokenRevokeResponse RevokeToken(TokenRevokeRequest revokeRequest);
    }
}
