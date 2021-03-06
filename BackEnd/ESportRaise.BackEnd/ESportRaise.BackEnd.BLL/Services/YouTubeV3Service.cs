﻿using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using ESportRaise.BackEnd.BLL.DTOs.YouTube;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class YouTubeV3Service : IYouTubeService
    {
        private const string BASE_API_URL = "https://www.googleapis.com/youtube/v3";

        private readonly string apiKey;

        public YouTubeV3Service(IConfiguration configuration)
        {
            apiKey = configuration.GetValue<string>("YouTubeApiKey");
        }

        #region YouTube API interaction

        private static async Task<JObject> SendApiRequestForUrlAsync(string url)
        {
            HttpWebRequest apiRequest = WebRequest.CreateHttp(url);
            apiRequest.Method = "GET";
            apiRequest.Accept = "application/json";
            apiRequest.ContentType = "application/json; charset=utf-8;";

            HttpWebResponse apiResponse = (await apiRequest.GetResponseAsync()) as HttpWebResponse;

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                using (var responseReader = new StreamReader(apiResponse.GetResponseStream()))
                {
                    string responseBody = await responseReader.ReadToEndAsync();
                    JsonReader reader = new JsonTextReader(new StringReader(responseBody));
                    reader.DateParseHandling = DateParseHandling.None;
                    return JObject.Load(reader);
                }
            }

            throw new BadRequestException($"Server responded with an error.");
        }

        #endregion

        #region Finding out user id

        public async Task<string> GetUserIdAsync(string channelUrl)
        {
            bool isLikeUserId = TryParseUserIdFromUrl(channelUrl, out string parsedUserId);
            if (isLikeUserId)
            {
                bool isVerifiedId = await IsUserRegisteredAsync(parsedUserId);
                if (isVerifiedId)
                {
                    return parsedUserId;
                }
            }

            bool isLikeUserName = TryParseUserNameFromUrl(channelUrl, out string parsedUserName);
            if (isLikeUserName)
            {
                string userId = await GetUserIdByUserNameAsync(parsedUserName);
                if (userId != null)
                {
                    return userId;
                }
            }

            throw new NotFoundException("Not valid or outdated channel URL is given!");
        }

        private static bool TryParseUserIdFromUrl(string channelUrl, out string userId)
        {
            const string channelIdUrl = "www.youtube.com/channel/";
            bool isChannelLinkWithId = channelUrl.Contains(channelIdUrl);
            if (isChannelLinkWithId)
            {
                int channelUrlBaseIndex = channelUrl.IndexOf(channelIdUrl);
                int userIdIndex = channelIdUrl.Length + channelUrlBaseIndex;
                userId = channelUrl.Substring(userIdIndex);
                return CanBeUserId(userId);
            }

            userId = null;
            return false;
        }

        private static bool CanBeUserId(string possibleUserId)
        {
            return new Regex(@"^UC[a-zA-Z0-9\-]+$").IsMatch(possibleUserId);
        }

        private async Task<bool> IsUserRegisteredAsync(string userId)
        {
            string idRequestUrl = $"{BASE_API_URL}/channels?part=id&id={userId}&key={apiKey}";

            JObject responseObject = await SendApiRequestForUrlAsync(idRequestUrl);
            JArray usersArray = responseObject["items"] as JArray;

            if (usersArray == null || usersArray.Count == 0)
            {
                return false;
            }

            string realUserId = usersArray[0]["id"].Value<string>();
            return realUserId == userId;
        }

        private static bool TryParseUserNameFromUrl(string channelUrl, out string userName)
        {
            string namedChannelLinkBase = "www.youtube.com/c/";
            bool isNamedChannelUrl = channelUrl.Contains(namedChannelLinkBase);
            if (!isNamedChannelUrl)
            {
                namedChannelLinkBase = "www.youtube.com/user/";
                if (!channelUrl.Contains(namedChannelLinkBase))
                {
                    userName = null;
                    return false;
                }
            }

            int namedChannelUrlIndex = channelUrl.IndexOf(namedChannelLinkBase);
            int userNameIndex = namedChannelLinkBase.Length + namedChannelUrlIndex;
            userName = channelUrl.Substring(userNameIndex);
            return CanBeUserName(userName);
        }

        private static bool CanBeUserName(string possibleUserName)
        {
            return new Regex(@"^[a-zA-Z0-9\-]+$").IsMatch(possibleUserName);
        }

        private async Task<string> GetUserIdByUserNameAsync(string userName)
        {
            string idRequestUrl = $"{BASE_API_URL}/channels?part=id&forUsername={userName}&key={apiKey}";

            JObject responseObject = await SendApiRequestForUrlAsync(idRequestUrl);
            JArray usersArray = responseObject["items"] as JArray;

            if (usersArray == null || usersArray.Count == 0)
            {
                return null;
            }

            string userId = usersArray[0]["id"].Value<string>();
            return userId;
        }

        #endregion

        public async Task<LiveStreamResponseDTO> GetCurrentLiveStreamAsync(LiveStreamRequestDTO request)
        {
            string userId = request.LiveStreamingServiceUserId;
            string liveStreamsUrl = $"{BASE_API_URL}/search?channelId={userId}&eventType=live&type=video&key={apiKey}";
            JObject result = await SendApiRequestForUrlAsync(liveStreamsUrl);

            JArray liveStreams = result["items"] as JArray;
            if (liveStreams == null || liveStreams.Count == 0)
            {
                return new LiveStreamResponseDTO();
            }

            string liveStreamId = liveStreams[0]["id"]["videoId"].Value<string>();
            StreamInfo liveStreamInfo = await GetVideoStreamInfo(liveStreamId);
            return new LiveStreamResponseDTO
            {
                LiveStreamId = liveStreamId,
                StartTime = liveStreamInfo.StartTime,
                EndTime = liveStreamInfo.EndTime
            };
        }

        public async Task<StreamInfo> GetVideoStreamInfo(string videoId)
        {
            string liveStreamUrl = $"{BASE_API_URL}/videos?part=liveStreamingDetails&id={videoId}&key={apiKey}";
            JObject result = await SendApiRequestForUrlAsync(liveStreamUrl);

            JArray videosInfo = result["items"] as JArray;
            if (videosInfo == null || videosInfo.Count == 0)
            {
                return null;
            }

            var liveStreamInfo = videosInfo[0]["liveStreamingDetails"];

            var startTimeKey = liveStreamInfo["actualStartTime"];
            if (startTimeKey == null)
            {
                return new StreamInfo
                {
                    Id = videoId
                };
            }
            string startTimeUtc = startTimeKey.Value<string>();

            var endTimeKey = liveStreamInfo["actualEndTime"];
            if (endTimeKey == null)
            {
                return new StreamInfo
                {
                    Id = videoId,
                    StartTime = startTimeUtc
                };
            }
            string endTimeUtc = endTimeKey.Value<string>();

            return new StreamInfo
            {
                Id = videoId,
                StartTime = startTimeUtc,
                EndTime = endTimeUtc
            };
        }
    }
}
