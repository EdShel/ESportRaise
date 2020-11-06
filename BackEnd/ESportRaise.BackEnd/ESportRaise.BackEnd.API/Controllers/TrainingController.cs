using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class TrainingController : Controller
    {
        private readonly TrainingService trainingService;

        private readonly TeamMemberService teamMemberService;

        public TrainingController(TrainingService trainingService, TeamMemberService teamMemberService)
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

        [HttpGet("broadcast")]
        public async Task<IActionResult> GetVideoStreamsForTraining(int trainingId)
        {
            IEnumerable<VideoStreamDTO> videoStreams = await trainingService.GetVideoStreamsAsync(trainingId);
            return new JsonResult(new
            {
                TrainingId = trainingId,
                Streams = videoStreams.Select(stream => new
                {
                    stream.Id,
                    stream.TeamMemberId,
                    stream.StreamId
                })
            });
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLastTrainingForTeam(int id)
        {
            await ValidateAccessToTeam(id);

            TrainingDTO training = await trainingService.GetCurrentTrainingForTeamAsync(id);
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
