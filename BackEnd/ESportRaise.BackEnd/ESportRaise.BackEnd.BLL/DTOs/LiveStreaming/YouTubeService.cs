using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.DTOs.LiveStreaming
{
    public class RetrieveIdServiceRequest
    {
        public string ChannelUrl { get; set; }
    }

    public class RetrieveIdServiceResponse
    {
        public string LiveStreamingServiceUserId { get; set; }
    }

    public class LiveStreamServiceRequest
    {
        public string LiveStreamingServiceUserId { get; set; }
    }

    public class LiveStreamServiceResponse
    {
        public string LiveStreamId { get; set; }
    }

}
