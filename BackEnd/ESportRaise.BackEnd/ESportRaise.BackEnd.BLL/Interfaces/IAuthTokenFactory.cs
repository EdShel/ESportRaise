using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IAuthTokenFactory
    {
        string GenerateTokenForClaims(IEnumerable<Claim> claims);
    }

    public interface IRefreshTokenFactory
    {
        string GenerateToken();
    }

    public interface IStreamingApiService
    {
        Task<RetrieveIdServiceResponse> GetUserId(RetrieveIdServiceRequest request);

        Task<LiveStreamServiceResponse> GetCurrentLiveStream(LiveStreamServiceRequest request);
    }
}
