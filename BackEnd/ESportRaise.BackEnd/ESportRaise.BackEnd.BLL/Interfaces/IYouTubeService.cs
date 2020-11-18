using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using ESportRaise.BackEnd.BLL.DTOs.YouTube;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IYouTubeService
    {
        Task<string> GetUserIdAsync(string channelUrl);

        Task<LiveStreamResponseDTO> GetCurrentLiveStreamAsync(LiveStreamRequestDTO request);

        Task<StreamInfo> GetVideoStreamInfo(string videoId);
    }
}
