using Microsoft.AspNetCore.Mvc;

namespace ESportRaise.FrontEnd.WEB.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index(int? id)
        {
            return View(id.HasValue ? id : -1);
        }
    }
}
