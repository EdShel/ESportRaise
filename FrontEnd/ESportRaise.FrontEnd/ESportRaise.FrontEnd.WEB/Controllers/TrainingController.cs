using ESportRaise.FrontEnd.WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESportRaise.FrontEnd.WEB.Controllers
{
    public class TrainingController : Controller
    {
        public IActionResult Index(int id, int? cid)
        {
            return View(new TrainingViewModel
            {
                TrainingId = id,
                CriticalMomentId = cid.HasValue ? cid.Value : -1
            });
        }
    }
}
