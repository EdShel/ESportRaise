using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IStreamingApiService
    {
        Task<RetrieveIdServiceResponse> GetUserId(RetrieveIdServiceRequest request);

        Task<LiveStreamServiceResponse> GetCurrentLiveStream(LiveStreamServiceRequest request);
    }
}
