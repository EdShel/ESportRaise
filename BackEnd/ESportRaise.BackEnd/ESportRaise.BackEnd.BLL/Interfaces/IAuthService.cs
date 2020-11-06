using ESportRaise.BackEnd.BLL.DTOs.Auth;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IAuthAsyncService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);

        Task RegisterAsync(RegisterDTO registerRequest);

        Task<TokenRefreshResponseDTO> RefreshTokenAsync(TokenRefreshRequestDTO refreshRequest);

        Task RevokeTokenAsync(TokenRevokeDTO revokeRequest);
    }
}
