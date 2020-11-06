using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IStreamingApiService
    {
        Task<string> GetUserId(string channelUrl);

        Task<LiveStreamResponseDTO> GetCurrentLiveStream(LiveStreamRequestDTO request);
    }
}
