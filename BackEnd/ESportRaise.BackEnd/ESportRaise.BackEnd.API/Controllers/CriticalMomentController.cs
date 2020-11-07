using ESportRaise.BackEnd.BLL.DTOs.CriticalMoment;
using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
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
        private readonly ICriticalMomentService criticalMomentService;

        private readonly ITrainingService trainingService;

        private readonly ITeamMemberService teamMemberService;

        public CriticalMomentController(
            ICriticalMomentService criticalMomentService, 
            ITrainingService trainingService, 
            ITeamMemberService teamMemberService)
        {
            this.criticalMomentService = criticalMomentService;
            this.trainingService = trainingService;
            this.teamMemberService = teamMemberService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetTrainingCriticalMomentsAsync(int id)
        {
            TrainingDTO training = await trainingService.GetTrainingAsync(id);
            if (training == null)
            {
                throw new NotFoundException("Training doesn't exist!");
            }
            await ValidateAccessToTeamAsync(training.TeamId);

            IEnumerable<CriticalMomentDTO> moments = await criticalMomentService
                .GetCriticalMomentsForTrainingAsync(id);

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

        private async Task ValidateAccessToTeamAsync(int teamId)
        {
            if (!await User.IsAuthorizedToAccessTeamAsync(teamId, teamMemberService))
            {
                throw new ForbiddenException("Not allowed to access team's critical moments!");
            }
        }
    }
}
