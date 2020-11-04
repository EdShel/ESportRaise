using ESportRaise.BackEnd.API.Models.Auth;
using ESportRaise.BackEnd.BLL.DTOs.Auth;
using ESportRaise.BackEnd.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{

    [Route("[controller]"), ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthAsyncService authService;

        private IStreamingApiService streamingApiService;

        public AuthController(IAuthAsyncService authService, IStreamingApiService streamingApiService)
        {
            this.authService = authService;
            this.streamingApiService = streamingApiService;
        }

        // TODO: allow Admin registration only by admins
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterServiceRequest request)
        {
            RegisterServiceResponse response = await authService.RegisterAsync(request);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginServiceRequest request)
        {
            LoginServiceResponse response = await authService.LoginAsync(request);

            return Ok(response);
        }

        // TODO: check whether User.Identity is accessible after the token has expired
        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] TokenRefreshAPIRequest request)
        {
            var requestDTO = new TokenServiceRefreshRequest
            {
                RefreshToken = request.RefreshToken,
                UserName = User.Identity.Name
            };
            TokenServiceRefreshResponse response = await authService.RefreshTokenAsync(requestDTO);

            return Ok(response);
        }

        [HttpPost("revoke"), Authorize]
        public async Task<ActionResult> RevokeToken([FromBody] TokenRevokeAPIRequest request)
        {
            var requestDTO = new TokenServiceRevokeRequest
            {
                RefreshToken = request.RefreshToken,
                UserName = User.Identity.Name
            };
            TokenServiceRevokeResponse response = await authService.RevokeTokenAsync(requestDTO);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public string Check()
        {
            return "You're allowed to see it";
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test(string url)
        {
            BLL.DTOs.LiveStreaming.RetrieveIdServiceResponse r = await streamingApiService.GetUserId(new BLL.DTOs.LiveStreaming.RetrieveIdServiceRequest
            {
                ChannelUrl = url
            });

            return new JsonResult(r);
        }

        [HttpGet("live")]
        public async Task<IActionResult> Live(string userId)
        {
            BLL.DTOs.LiveStreaming.LiveStreamServiceResponse r = await streamingApiService.GetCurrentLiveStream(new BLL.DTOs.LiveStreaming.LiveStreamServiceRequest
            {
                LiveStreamingServiceUserId = userId
            });

            return new JsonResult(r);
        }
    }
}
