using Microsoft.AspNetCore.Mvc;

namespace ESportRaise.FrontEnd.WEB.Controllers
{
    public class TrainingController : Controller
    {
        public IActionResult Index(int id)
        {
            return View(id);
        }
    }
}
