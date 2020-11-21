using System;
using System.Net;
using System.Threading.Tasks;

namespace ESportRaise.FrontEnd.WEB.Services
{
    public class CertExpirationChecker
    {
        public async Task<DateTime?> GetCertExpirationDateAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            DateTime expiration = default;
            request.ServerCertificateValidationCallback += (sender, cert, chain, errors) =>
            {
                expiration = DateTime.Parse(cert.GetExpirationDateString());
                return true;
            };
            try
            {
                using (_ = await request.GetResponseAsync())
                {
                    return expiration;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
