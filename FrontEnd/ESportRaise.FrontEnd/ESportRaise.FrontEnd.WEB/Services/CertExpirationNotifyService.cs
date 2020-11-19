using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESportRaise.FrontEnd.WEB.Services
{
    public class CertExpirationNotifyService : HostedService
    {
        private readonly MonitoringOptions options;

        private readonly CertExpirationChecker expirationChecker;

        private readonly EmailService emailService;

        public CertExpirationNotifyService(
            IConfiguration config, 
            CertExpirationChecker expirationChecker, 
            EmailService emailService)
        {
            options = new MonitoringOptions();
            config.Bind("SslMonitoring", options);
            this.expirationChecker = expirationChecker;
            this.emailService = emailService;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!options.Enabled)
            {
                return;
            }
            do
            {
                DateTime? expires = await expirationChecker.GetCertExpirationDateAsync(options.Host);
                bool expired = expires.HasValue
                    && (expires.Value - DateTime.UtcNow).TotalMinutes <= options.MinutesUntilExpiration;
                if (expired)
                {
                    await emailService.SendAsync(
                        options.EmailForNotifications,
                        "ESport Raise SSL expiration",
                        $"The certificate will expire at {expires.Value}");

                    await Task.Delay(options.NotificationDelayMinutes * 60 * 1000, cancellationToken);
                }
                else
                {
                    await Task.Delay(options.PeriodMinutes * 60 * 1000, cancellationToken);
                }
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        private class MonitoringOptions
        {
            public bool Enabled { get; set; }

            public int PeriodMinutes { get; set; }

            public int MinutesUntilExpiration { get; set; }

            public string Host { get; set; }

            public int NotificationDelayMinutes { get; set; }

            public string EmailForNotifications { get; set; }
        }
    }
}
