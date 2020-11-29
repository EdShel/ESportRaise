using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class KeyController : Controller
    {
        private readonly IConfiguration config;

        public KeyController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpGet("youtube")]
        public IActionResult GetYouTubeKey()
        {
            return Json(new
            {
                Key = config.GetValue<string>("YouTubePlayerKey")
            });
        }
    }
}
