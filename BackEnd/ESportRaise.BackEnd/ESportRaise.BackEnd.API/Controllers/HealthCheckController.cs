using Microsoft.AspNetCore.Mvc;
using System;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check()
        {
            return Ok(DateTime.Now);
        }
    }
}
