using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.BLL.Services;
using ESportRaise.BackEnd.BLL.Constants;
using Microsoft.AspNetCore.Mvc.Filters;
using ESportRaise.BackEnd.BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class TrainingController : Controller
    {
        private readonly ITrainingService trainingService;

        private readonly TeamMemberService teamMemberService;

        public TrainingController(ITrainingService trainingService, TeamMemberService teamMemberService)
        {
            this.trainingService = trainingService;
            this.teamMemberService = teamMemberService;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate()
        {
            int userId = User.GetUserId();
            var request = new InitiateTrainingServiceRequest { UserId = userId };
            var response = await trainingService.InitiateTrainingAsync(request);
            return new JsonResult(new
            {
                TrainingId = response.TrainingId
            });
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLastTrainingForTeam(int id)
        {
            await ValidateAccessToTeam(id);

            TrainingDTO training = await trainingService.GetLastTrainingForTeamAsync(id);
            return new JsonResult(new
            {
                training.Id,
                training.TeamId,
                training.BeginTime
            });
        }

        [HttpGet("beforeDay")]
        public async Task<IActionResult> GetTrainingAtDay(int id, DateTime dateTime, int hours)
        {
            await ValidateAccessToTeam(id);

            IEnumerable<TrainingDTO> trainings = await trainingService.GetTrainingsBeforeDateTime(id, dateTime, hours);
            return new JsonResult(new
            {
                TeamId = id,
                Trainings = trainings.Select(training => new
                {
                    training.Id,
                    training.BeginTime
                })
            });
        }

        private async Task ValidateAccessToTeam(int teamId)
        {
            if (!await User.IsAuthorizedToAccessTeamAsync(teamId, teamMemberService))
            {
                throw new ForbiddenException("Not allowed to access team!");
            }
        }
    }
}
