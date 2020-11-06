using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IStreamingApiService
    {
        Task<string> GetUserIdAsync(string channelUrl);

        Task<LiveStreamResponseDTO> GetCurrentLiveStreamAsync(LiveStreamRequestDTO request);
    }
}
