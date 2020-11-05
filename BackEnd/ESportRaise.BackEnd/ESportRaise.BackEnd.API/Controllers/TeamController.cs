﻿using ESportRaise.BackEnd.API.Models.Team;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.DTOs.Team;
using ESportRaise.BackEnd.BLL.Services;
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