using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ESportRaise.FrontEnd.WEB.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Cert()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://google.com");
            X509Certificate cert2 = null;
            HttpWebResponse response = null;

            request.ServerCertificateValidationCallback += CertCheck;
            using(var x = request.GetResponse())
            {

            }

            return Ok();
        }

        private bool CertCheck(
            object sender, X509Certificate certificate, X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine(certificate);
            certificate.GetExpirationDateString();
            return true;
        }
    }
}
