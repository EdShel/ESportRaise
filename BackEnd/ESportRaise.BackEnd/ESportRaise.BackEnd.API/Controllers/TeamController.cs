using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using ESportRaise.BackEnd.API.Models.Team;
using ESportRaise.BackEnd.BLL.DTOs.Team;
using ESportRaise.BackEnd.BLL.Services;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.API.Models.TeamMember;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.BLL.DTOs.LiveStreaming;

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

    [Route("[controller]"), ApiController, Authorize]
    public class TeamController : ControllerBase
    {
        private readonly TeamService teamService;

        private readonly TeamMemberService teamMemberService;

        private readonly AppUserService users;

        public TeamController(TeamService teamService, TeamMemberService teamMemberService, AppUserService users)
        {
            this.teamService = teamService;
            this.teamMemberService = teamMemberService;
            this.users = users;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateRequest request)
        {
            CreateTeamDTO serviceRequest = new CreateTeamDTO
            {
                CreatorId = User.GetUserId(),
                Name = request.TeamName
            };
            int teamId = await teamService.CreateTeamAsync(serviceRequest);
            return new JsonResult(new
            {
                TeamId = teamId
            });
        }

        [HttpPost("addMember")]
        public async Task<IActionResult> AddTeamMember([FromBody] AddTeamMemberRequest request)
        {
            AppUserDTO newTeamMember = await users.GetUserAsync(request.User);
            await teamService.AddTeamMemberAsync(request.TeamId, newTeamMember.Id);
            return Ok();
        }

        [HttpPost("removeMember")]
        public async Task<IActionResult> RemoveTeamMember([FromBody] RemoveTeamMemberRequest request)
        {
            AppUserDTO deletedMember = await users.GetUserAsync(request.User);
            await teamService.RemoveTeamMemberAsync(request.TeamId, deletedMember.Id);
            return Ok();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFullTeam(int id)
        {
            TeamDTO team = await teamService.GetTeamAsync(id);
            return new JsonResult(new
            {
                Id = team.Id,
                Name = team.Name,
                Members = team.Members.Select(member => new
                {
                    Id = member.Id,
                    Name = member.UserName
                })
            });
        }
    }
}
