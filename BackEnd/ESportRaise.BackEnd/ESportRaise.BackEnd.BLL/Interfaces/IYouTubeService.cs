using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IYouTubeService
    {
        Task<string> GetUserIdAsync(string channelUrl);

        Task<LiveStreamResponseDTO> GetCurrentLiveStreamAsync(LiveStreamRequestDTO request);
    }
}
