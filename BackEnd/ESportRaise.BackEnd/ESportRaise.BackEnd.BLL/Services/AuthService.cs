using ESportRaise.BackEnd.BLL.DTOs.Auth;
using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class YouTubeV3Service : IStreamingApiService
    {
        private const string BASE_API_URL = "https://www.googleapis.com/youtube/v3";

        private readonly string apiKey;

        public YouTubeV3Service(IConfiguration configuration)
        {
            apiKey = configuration.GetValue<string>("YouTubeApiKey");
        }

        #region YouTube API interaction

        private static async Task<JObject> GetJsonBodyForRequest(string url)
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
                    return JObject.Parse(responseBody);
                }
            }

            return null;
        }

        #endregion

        #region Finding out user id

        public async Task<RetrieveIdServiceResponse> GetUserId(RetrieveIdServiceRequest request)
        {
            string channelUrl = request.ChannelUrl;
            bool isLikeUserId = TryParseUserIdFromUrl(channelUrl, out string parsedUserId);
            if (isLikeUserId)
            {
                bool isVerifiedId = await IsUserRegistered(parsedUserId);
                if (isVerifiedId)
                {
                    return new RetrieveIdServiceResponse { LiveStreamingServiceUserId = parsedUserId };
                }
            }

            bool isLikeUserName = TryParseUserNameFromUrl(channelUrl, out string parsedUserName);
            if (isLikeUserName)
            {
                string userId = await GetUserIdByUserName(parsedUserName);
                if (userId != null)
                {
                    return new RetrieveIdServiceResponse { LiveStreamingServiceUserId = userId };
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

        private async Task<bool> IsUserRegistered(string userId)
        {
            string idRequestUrl = $"{BASE_API_URL}/channels?part=id&id={userId}&key={apiKey}";

            JObject responseObject = await GetJsonBodyForRequest(idRequestUrl);
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

        private async Task<string> GetUserIdByUserName(string userName)
        {
            string idRequestUrl = $"{BASE_API_URL}/channels?part=id&forUsername={userName}&key={apiKey}";

            JObject responseObject = await GetJsonBodyForRequest(idRequestUrl);
            JArray usersArray = responseObject["items"] as JArray;

            if (usersArray == null || usersArray.Count == 0)
            {
                return null;
            }

            string userId = usersArray[0]["id"].Value<string>();
            return userId;
        }

        #endregion

        public async Task<LiveStreamServiceResponse> GetCurrentLiveStream(LiveStreamServiceRequest request)
        {
            throw new NotImplementedException();
        }
    }

    public class AuthService : IAuthAsyncService
    {
        private UserAsyncRepository usersRepository;

        private IAuthTokenFactory tokenFactory;

        private IRefreshTokenFactory refreshTokenFactory;

        public AuthService(UserAsyncRepository users, IAuthTokenFactory tokenFactory, IRefreshTokenFactory refreshTokenFactory)
        {
            this.usersRepository = users;
            this.tokenFactory = tokenFactory;
            this.refreshTokenFactory = refreshTokenFactory;
        }

        public async Task<LoginServiceResponse> LoginAsync(LoginServiceRequest loginRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByEmailOrUserNameAsync(loginRequest.EmailOrUserName);
            if (user != null && usersRepository.IsUserPasswordCorrect(user, loginRequest.Password))
            {
                var tokenClaims = GetTokenClaimsForUser(user);
                var refreshToken = refreshTokenFactory.GenerateToken();
                await usersRepository.CreateRefreshTokenAsync(user, refreshToken);

                return new LoginServiceResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = tokenFactory.GenerateTokenForClaims(tokenClaims),
                    RefreshToken = refreshToken
                };
            }
            throw new BadRequestException("Email, user name or password is incorrect!");
        }

        private IEnumerable<Claim> GetTokenClaimsForUser(AppUser user)
        {
            var userClaims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole)
            };
            return userClaims;
        }

        public async Task<RegisterServiceResponse> RegisterAsync(RegisterServiceRequest registerRequest)
        {
            bool emailIsTaken = await usersRepository.IsAnyUserWithEmailAsync(registerRequest.Email);
            if (emailIsTaken)
            {
                throw new BadRequestException("Email is taken!");
            }

            bool nameIsTaken = await usersRepository.IsAnyUserWithUserNameAsync(registerRequest.UserName);
            if (nameIsTaken)
            {
                throw new BadRequestException("User name is taken!");
            }

            var user = new AppUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                UserRole = registerRequest.Role
            };
            await usersRepository.CreateAsync(user, registerRequest.Password);

            return new RegisterServiceResponse();
        }

        public async Task<TokenServiceRefreshResponse> RefreshTokenAsync(TokenServiceRefreshRequest refreshRequest)
        {
            AppUser user = await usersRepository.GetUserOrDefaultByUserNameAsync(refreshRequest.UserName);
            if (user == null)
            {
                throw new BadRequestException("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshToken(user, refreshRequest.RefreshToken);
            if (!validRefreshToken)
            {
                throw new BadRequestException("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, refreshRequest.RefreshToken);

            var newRefreshToken = refreshTokenFactory.GenerateToken();
            await usersRepository.CreateRefreshTokenAsync(user, newRefreshToken);

            var userClaims = GetTokenClaimsForUser(user);
            return new TokenServiceRefreshResponse
            {
                Token = tokenFactory.GenerateTokenForClaims(userClaims),
                RefreshToken = newRefreshToken
            };
        }

        public async Task<TokenServiceRevokeResponse> RevokeTokenAsync(TokenServiceRevokeRequest revokeRequest)
        {
            var user = await usersRepository.GetUserOrDefaultByUserNameAsync(revokeRequest.UserName);
            if (user == null)
            {
                throw new BadRequestException("Not valid user!");
            }

            bool validRefreshToken = await usersRepository.HasRefreshToken(user, revokeRequest.RefreshToken);
            if (!validRefreshToken)
            {
                throw new BadRequestException("Not valid refresh token!");
            }

            await usersRepository.DeleteRefreshTokenAsync(user, revokeRequest.RefreshToken);

            return new TokenServiceRevokeResponse();
        }
    }
}
