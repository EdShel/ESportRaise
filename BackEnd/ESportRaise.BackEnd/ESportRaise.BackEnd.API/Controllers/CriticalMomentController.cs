using ESportRaise.BackEnd.BLL.DTOs.CriticalMoment;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class CriticalMomentController : Controller
    {
        private readonly CriticalMomentService criticalMomentService;

        private readonly TeamMemberService teamMemberService;

        public CriticalMomentController(CriticalMomentService criticalMomentService, TeamMemberService teamMemberService)
        {
            this.criticalMomentService = criticalMomentService;
            this.teamMemberService = teamMemberService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainingCriticalMoments(int id)
        {
            IEnumerable<CriticalMomentDTO> moments = await criticalMomentService.GetCriticalMomentsForTrainingAsync(id);
            return new JsonResult(new
            {
                TeamId = id,
                TrainingId = moments.FirstOrDefault()?.TrainingId ?? -1,
                Moments = moments.Select(moment => new
                {
                    Begin = moment.BeginTime,
                    End = moment.EndTime
                })
            });
        }

        private async Task ValidateAccessToTeam(int teamId)
        {
            if (!await User.IsAuthorizedToAccessTeamAsync(teamId, teamMemberService))
            {
                throw new ForbiddenException("Not allowed to access team's critical moments!");
            }
        }
    }
}
