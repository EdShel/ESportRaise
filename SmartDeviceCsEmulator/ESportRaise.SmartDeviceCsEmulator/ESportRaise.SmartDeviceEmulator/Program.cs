using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESportRaise.SmartDeviceEmulator
{
    class Program
    {
        private static string backendUrl;

        private static HttpClient client;

        static void Main(string[] args)
        {
            backendUrl = args[0];
            client = GetUntrustedCertHttpClient();

            string authToken = GetAuthToken();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);

            int trainingId = InitiateTraining();
            Console.WriteLine($"Starting training #{trainingId}");

            var cts = new CancellationTokenSource();
            Task.Run(() => SendPhysicalState(trainingId, cts.Token));

            Console.WriteLine("Press ENTER to stop sending physical state");
            Console.ReadLine();
            cts.Cancel();
        }

        private static HttpClient GetUntrustedCertHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
            return new HttpClient(handler);
        }

        private static string GetAuthToken()
        {
            const string userName = "Eduardo";
            const string password = "Qwerty12345@";

            string requestBodyJson = JsonConvert.SerializeObject(new
            {
                emailOrUserName = userName,
                password = password
            });
            HttpRequestMessage authRequest = new HttpRequestMessage(HttpMethod.Post, $"{backendUrl}/auth/login")
            {
                Content = new StringContent(requestBodyJson, Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            var response = client.SendAsync(authRequest).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var bodyJson = JObject.Parse(body);
            return bodyJson["token"].Value<string>();
        }

        private static int InitiateTraining()
        {
            HttpRequestMessage authRequest = new HttpRequestMessage(
                HttpMethod.Post, $"{backendUrl}/training/initiate");
            var response = client.SendAsync(authRequest).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var bodyJson = JObject.Parse(body);
            return bodyJson["trainingId"].Value<int>();
        }

        private static void SendPhysicalState(int trainingId, CancellationToken ct)
        {
            const int sendingDelayMsecs = 5 * 1000;

            const int heartrate = 90;
            const int heartrateAmplitude = 40;
            const float temperature = 36.6f;
            const float temperatureAmplitude = 3f;

            const int sendingsBeforeRepeat = 30;
            const double step = Math.PI / sendingsBeforeRepeat;
            double currentTime = 0d;

            while (!ct.IsCancellationRequested)
            {
                Task.Delay(sendingDelayMsecs, ct).Wait();

                var stateRecordObject = new
                {
                    trainingId = trainingId,
                    heartrate = heartrate + (int)(Math.Max(0, Math.Sin(currentTime)) * heartrateAmplitude),
                    temperature = temperature + (float)(Math.Max(0, Math.Sin(currentTime)) * temperatureAmplitude)
                };

                Console.Write($"Sending HR: {stateRecordObject.heartrate}, t: {stateRecordObject.temperature} ");

                currentTime += step;

                var request = new HttpRequestMessage(
                    HttpMethod.Post, $"{backendUrl}/stateRecord/send")
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(stateRecordObject),
                        Encoding.UTF8,
                        MediaTypeNames.Application.Json)
                };

                var response = client.SendAsync(request, ct).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("OK");
                }
                else
                {
                    Console.WriteLine($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
                }
            }
        }
    }
}
