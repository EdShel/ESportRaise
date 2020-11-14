using Microsoft.AspNetCore.Mvc;

namespace ESportRaise.FrontEnd.WEB.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index(int? id)
        {
            return View(id.HasValue ? id : -1);
        }
    }
}
