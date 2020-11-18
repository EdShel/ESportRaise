using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
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
        private readonly ITrainingService trainingService;

        private readonly ITeamMemberService teamMemberService;

        public TrainingController(ITrainingService trainingService, ITeamMemberService teamMemberService)
        {
            this.trainingService = trainingService;
            this.teamMemberService = teamMemberService;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiateAsync()
        {
            int userId = User.GetUserId();
            int trainingId = await trainingService.InitiateTrainingAsync(userId);
            return new JsonResult(new
            {
                TrainingId = trainingId
            });
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopAsync()
        {
            int userId = User.GetUserId();
            int teamId = await teamMemberService.GetTeamIdAsync(userId);
            var training = await trainingService.GetCurrentTrainingForTeamAsync(teamId);
            if (training == null)
            {
                throw new BadRequestException("Your team doesn't have trainings now!");
            }

            await trainingService.StopTrainingAsync(training.Id);
            return Ok();
        }

        [HttpGet("broadcast")]
        public async Task<IActionResult> GetVideoStreamsForTraining(int id)
        {
            await ValidateAccessToTraining(id);

            IEnumerable<VideoStreamDTO> videoStreams = await trainingService.GetVideoStreamsAsync(id);
            return new JsonResult(new
            {
                TrainingId = id,
                Streams = videoStreams.Select(stream => new
                {
                    stream.Id,
                    stream.TeamMemberId,
                    stream.StreamId,
                    stream.StartTime,
                    stream.EndTime
                })
            });
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetLastTrainingForTeamAsync(int id)
        {
            await ValidateAccessToTeamAsync(id);

            TrainingDTO training = await trainingService.GetCurrentTrainingForTeamAsync(id);
            return new JsonResult(new
            {
                training.Id,
                training.TeamId,
                training.BeginTime,
            });
        }

        [HttpGet("beforeDay")]
        public async Task<IActionResult> GetTrainingAtDayAsync(int id, DateTime dateTime, int hours)
        {
            await ValidateAccessToTeamAsync(id);

            IEnumerable<TrainingDTO> trainings = await trainingService.GetTrainingsBeforeDateTimeAsync(id, dateTime, hours);
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

        [HttpGet]
        public async Task<IActionResult> GetTrainingInfoAsync(int id)
        {
            await ValidateAccessToTeamAsync(id);

            TrainingDTO training = await trainingService.GetTrainingAsync(id);
            return Ok(new
            {
                training.Id,
                training.TeamId,
                training.BeginTime
            });
        }

        private async Task ValidateAccessToTraining(int trainingId)
        {
            TrainingDTO training = await trainingService.GetTrainingAsync(trainingId);
            if (training == null)
            {
                throw new BadRequestException("Invalid training id!");
            }

            await ValidateAccessToTeamAsync(training.TeamId);
        }

        private async Task ValidateAccessToTeamAsync(int teamId)
        {
            if (!await User.IsAuthorizedToAccessTeamAsync(teamId, teamMemberService))
            {
                throw new ForbiddenException("Not allowed to access team's training!");
            }
        }
    }
}
