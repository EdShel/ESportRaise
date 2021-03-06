﻿using ESportRaise.BackEnd.API.Models.TeamMember;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.DTOs.TeamMember;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class TeamMemberController : ControllerBase
    {
        private readonly ITeamMemberService memberService;

        private readonly IYouTubeService youTubeService;

        public TeamMemberController(ITeamMemberService memberService, IYouTubeService youTubeService)
        {
            this.memberService = memberService;
            this.youTubeService = youTubeService;
        }

        [HttpPut("youTube")]
        public async Task<IActionResult> ChangeYouTubeAccount([FromBody] EditYouTubeIdRequest request)
        {
            int currentUserId = User.GetUserId();
            if (currentUserId != request.TeamMemberId && !User.IsInRole(AuthConstants.ADMIN_ROLE))
            {
                throw new ForbiddenException("Can't edit this user data!");
            }

            if (memberService.GetTeamMemberOrNullAsync(currentUserId) == null)
            {
                throw new BadRequestException("The user need to belong to a team first!");
            }
            string channelId;
            if (string.IsNullOrWhiteSpace(request.ChannelUrl))
            {
                channelId = null;
            }
            else
            {
                channelId = await youTubeService.GetUserIdAsync(request.ChannelUrl);
            }
            await memberService.ChangeYouTubeChannelIdAsync(request.TeamMemberId, channelId);

            return Ok();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentTeamMemberInfo()
        {
            TeamMemberDTO teamMember = await memberService.GetTeamMemberOrNullAsync(User.GetUserId());
            if (teamMember == null)
            {
                throw new BadRequestException("You don't have a team!");
            }

            return new JsonResult(new
            {
                UserId = teamMember.Id,
                TeamId = teamMember.TeamId,
                YouTubeId = teamMember.YouTubeId
            });
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAnotherTeamMemberInfo(int id)
        {
            TeamMemberDTO teamMember = await memberService.GetTeamMemberOrNullAsync(id);
            if (teamMember == null)
            {
                throw new BadRequestException("The user doesn't have a team!");
            }

            return new JsonResult(new
            {
                UserId = teamMember.Id,
                TeamId = teamMember.TeamId,
                YouTubeId = teamMember.YouTubeId
            });
        }
    }
}
