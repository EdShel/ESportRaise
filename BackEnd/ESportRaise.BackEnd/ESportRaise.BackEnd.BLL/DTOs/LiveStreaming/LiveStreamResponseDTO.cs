namespace ESportRaise.BackEnd.BLL.DTOs.LiveStreaming
{
    public class LiveStreamResponseDTO
    {
        public bool HasLivestream => !string.IsNullOrEmpty(LiveStreamId);

        public string LiveStreamId { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
