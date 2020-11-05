using Microsoft.AspNetCore.Mvc;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check()
        {
            return Ok();
        }
    }
}
