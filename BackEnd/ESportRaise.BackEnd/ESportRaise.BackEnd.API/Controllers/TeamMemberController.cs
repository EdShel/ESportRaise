using ESportRaise.BackEnd.API.Models.TeamMember;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;
using ESportRaise.BackEnd.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize(Roles = AuthConstants.MEMBER_ROLE)]
    public class TeamMemberController : ControllerBase
    {
        private readonly TeamMemberService memberService;

        private readonly YouTubeV3Service youTubeService;

        public TeamMemberController(TeamMemberService memberService)
        {
            this.memberService = memberService;
        }

        [HttpPut("youTube")]
        public async Task<IActionResult> ChangeYouTubeAccount([FromBody] EditYouTubeIdRequest request)
        {
            var apiIdRequest = new RetrieveIdServiceRequest
            {
                ChannelUrl = request.ChannelUrl
            };
            RetrieveIdServiceResponse apiIdResponse = await youTubeService.GetUserId(apiIdRequest);
            int userId = User.GetUserId();
            await memberService.ChangeYouTubeChannelId(userId, apiIdResponse.LiveStreamingServiceUserId);

            return Ok();
        }
    }
}
