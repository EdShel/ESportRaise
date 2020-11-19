using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace ESportRaise.FrontEnd.WEB.Services
{
    public class CertExpirationChecker
    {
        private ManualResetEvent certInfoEvent;

        public CertExpirationChecker()
        {
            certInfoEvent = new ManualResetEvent(false);
        }

        public async Task<DateTime?> GetCertExpirationDateAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            DateTime expiration = default;
            request.ServerCertificateValidationCallback += (sender, c, chain, errors) =>
            {
                expiration = DateTime.Parse(c.GetExpirationDateString());
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

        private bool CertCheck(
            object sender, 
            X509Certificate certificate, 
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            certInfoEvent.Set();
            certificate.GetExpirationDateString();
            return true;
        }
    }
}
