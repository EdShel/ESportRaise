using ESportRaise.BackEnd.BLL.DTOs.Auth;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IAuthAsyncService
    {
        Task<LoginServiceResponse> LoginAsync(LoginServiceRequest loginRequest);

        Task<RegisterServiceResponse> RegisterAsync(RegisterServiceRequest registerRequest);

        Task<TokenServiceRefreshResponse> RefreshTokenAsync(TokenServiceRefreshRequest refreshRequest);

        Task<TokenServiceRevokeResponse> RevokeTokenAsync(TokenServiceRevokeRequest revokeRequest);
    }
}
