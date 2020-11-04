using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ESportRaise.BackEnd.BLL.DTOs.Training;
using ESportRaise.BackEnd.BLL.Interfaces;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController]
    public class TrainingController : ControllerBase
    {
        private ITrainingService trainingService;

        public TrainingController(ITrainingService trainingService)
        {
            this.trainingService = trainingService;
        }

        [HttpPost("initiate"), Authorize]
        public async Task<IActionResult> Initiate()
        {
            int userId = User.GetUserId();
            var request = new InitiateTrainingServiceRequest { UserId = userId };
            var response = await trainingService.InitiateTraining(request);
            return new JsonResult(new
            {
                TrainingId = response.TrainingId
            });
        }

    }
}
