namespace ESportRaise.BackEnd.BLL.DTOs.LiveStreaming
{
    public class LiveStreamServiceResponse
    {
        public bool HasLivestream => !string.IsNullOrEmpty(LiveStreamId);

        public string LiveStreamId { get; set; }
    }

}
