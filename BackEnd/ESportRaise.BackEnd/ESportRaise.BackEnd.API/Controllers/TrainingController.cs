using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Interfaces;
using AutoMapper;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingService trainingService;

        private readonly Mapper mapper;

        public TrainingController(ITrainingService trainingService, Mapper mapper)
        {
            this.trainingService = trainingService;
            this.mapper = mapper;
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

        [HttpPost("stateRecord")]
        public async Task StateRecord([FromBody] StateRecordRequest request)
        {
            var serviceRequest = mapper.Map<SaveStateRecordServiceRequest>(request);
            await trainingService.SaveStateRecordAsync(serviceRequest);
        }
    }
}
