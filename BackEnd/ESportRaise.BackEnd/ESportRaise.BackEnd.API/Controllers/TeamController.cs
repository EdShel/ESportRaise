using ESportRaise.BackEnd.API.Models.Team;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.DTOs.Team;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService teamService;

        private readonly ITeamMemberService teamMemberService;

        private readonly IAppUserService users;

        public TeamController(ITeamService teamService, ITeamMemberService teamMemberService, IAppUserService users)
        {
            this.teamService = teamService;
            this.teamMemberService = teamMemberService;
            this.users = users;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTeamAsync([FromBody] TeamCreateRequest request)
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
        public async Task<IActionResult> AddTeamMemberAsync([FromBody] AddTeamMemberRequest request)
        {
            await ValidateAccessToTeamAsync(request.TeamId);

            AppUserDTO newTeamMember = await users.GetUserAsync(request.User);
            await teamService.AddTeamMemberAsync(request.TeamId, newTeamMember.Id);
            return Ok();
        }

        [HttpPost("removeMember")]
        public async Task<IActionResult> RemoveTeamMemberAsync([FromBody] RemoveTeamMemberRequest request)
        {
            await ValidateAccessToTeamAsync(request.TeamId);

            AppUserDTO deletedMember = await users.GetUserAsync(request.User);
            await teamService.RemoveTeamMemberAsync(request.TeamId, deletedMember.Id);
            return Ok();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFullTeamAsync(int id)
        {
            await ValidateAccessToTeamAsync(id);

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

        private async Task ValidateAccessToTeamAsync(int teamId)
        {
            if (!await User.IsAuthorizedToAccessTeamAsync(teamId, teamMemberService))
            {
                throw new ForbiddenException("Not allowed to access the team!");
            }
        }
    }
}
