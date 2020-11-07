using ESportRaise.BackEnd.API.Models.StateRecord;
using ESportRaise.BackEnd.BLL.DTOs.StateRecord;
using ESportRaise.BackEnd.BLL.DTOs.Training;
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
    public class StateRecordController : ControllerBase
    {
        private readonly IStateRecordService stateRecordService;

        private readonly ITrainingService trainingService;

        private readonly ITeamMemberService teamMemberService;

        public StateRecordController(IStateRecordService stateRecordService, ITrainingService trainingService, ITeamMemberService teamMemberService)
        {
            this.stateRecordService = stateRecordService;
            this.trainingService = trainingService;
            this.teamMemberService = teamMemberService;
        }

        [HttpPost("send")]
        public async Task StateRecordAsync([FromBody] SaveStateRecordRequest request)
        {
            await ValidateAccessToTraining(request.TrainingId);

            int userId = User.GetUserId();
            var serviceRequest = new StateRecordDTO
            {
                TeamMemberId = userId,
                TrainingId = request.TrainingId,
                HeartRate = request.HeartRate,
                Temperature = request.Temperature
            };
            await stateRecordService.SaveStateRecordAsync(serviceRequest);
        }

        [HttpGet("last")]
        public async Task<IActionResult> GetRecordsForTrainingMostRecentAsync(int trainingId, int timeInSecs, int? userId)
        {
            await ValidateAccessToTraining(trainingId);

            var serviceRequest = new StateRecordRequestDTO
            {
                TeamMemberId = userId,
                TrainingId = trainingId,
                TimeInSeconds = timeInSecs
            };
            var serviceResponse = await stateRecordService.GetRecentAsync(serviceRequest);
            return new JsonResult(new
            {
                TrainingId = serviceResponse.TrainingId,
                Records = serviceResponse.StateRecords.Select(rec => new
                {
                    rec.TeamMemberId,
                    rec.CreateTime,
                    rec.HeartRate,
                    rec.Temperature
                })
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetRecordsForTrainingAsync(int trainingId)
        {
            await ValidateAccessToTraining(trainingId);

            var serviceResponse = await stateRecordService.GetForTrainingAsync(trainingId);
            return new JsonResult(new
            {
                TrainingId = serviceResponse.TrainingId,
                Records = serviceResponse.StateRecords.Select(rec => new
                {
                    rec.TeamMemberId,
                    rec.CreateTime,
                    rec.HeartRate,
                    rec.Temperature
                })
            });
        }

        private async Task ValidateAccessToTraining(int trainingId)
        {
            TrainingDTO training = await trainingService.GetTrainingAsync(trainingId);
            if (training == null)
            {
                throw new BadRequestException("Invalid training id!");
            }

            if (!await User.IsAuthorizedToAccessTeamAsync(training.TeamId, teamMemberService))
            {
                throw new ForbiddenException("Not allowed to access state of this team's players!");
            }
        }
    }
}
