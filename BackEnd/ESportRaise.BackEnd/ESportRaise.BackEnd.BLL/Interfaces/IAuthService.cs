using ESportRaise.BackEnd.BLL.DTOs.Tokens;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IAuthAsyncService
    {
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);

        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);

        Task<TokenRefreshResponse> RefreshTokenAsync(TokenRefreshRequest refreshRequest);

        Task<TokenRevokeResponse> RevokeTokenAsync(TokenRevokeRequest revokeRequest);
    }
}
