using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.API.Models.StateRecord;
using ESportRaise.BackEnd.BLL.DTOs.StateRecord;
using System.Linq;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class StateRecordController : ControllerBase
    {
        private readonly IStateRecordService stateRecordService;

        public StateRecordController(IStateRecordService stateRecordService)
        {
            this.stateRecordService = stateRecordService;
        }

        [HttpPost("stateRecord")]
        public async Task StateRecord([FromBody] SaveStateRecordRequest request)
        {
            int userId = User.GetUserId();
            // TODO: check if this user belongs to the team
            var serviceRequest = new SaveStateRecordServiceRequest
            {
                TeamMemberId = userId,
                TrainingId = request.TrainingId,
                HeartRate = request.HeartRate,
                Temperature = request.Temperature
            };
            await stateRecordService.SaveStateRecordAsync(serviceRequest);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetRecords([FromBody] GetStateRecordServiceRequest request)
        {
            // TODO: check access
            var serviceRequest = new GetStateRecordServiceRequest
            {
                TeamMemberId = request.TeamMemberId,
                TrainingId = request.TrainingId,
                TimeInSeconds = request.TimeInSeconds
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
    }
}
