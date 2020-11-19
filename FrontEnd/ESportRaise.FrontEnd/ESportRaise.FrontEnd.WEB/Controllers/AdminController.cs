using ESportRaise.FrontEnd.WEB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ESportRaise.FrontEnd.WEB.Controllers
{
    public class AdminController : Controller
    {
        private readonly CertExpirationChecker expirationChecker;

        private readonly string expirationCheckUrl;

        public AdminController(CertExpirationChecker expirationChecker, IConfiguration configuration)
        {
            this.expirationChecker = expirationChecker;
            this.expirationCheckUrl = configuration.GetValue<string>("SslMonitoring:Host"); 
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public async Task<IActionResult> SslExpiration()
        {
            DateTime? expirationDate = await expirationChecker
                .GetCertExpirationDateAsync(expirationCheckUrl);

            return Json(new
            {
                ExpiresAt = expirationDate
            });
        }
    }
}
