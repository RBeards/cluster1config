using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static string customerId = "381dff95-05e1-4c7f-836a-b140250ca8e2";

        // For sharedKey, use either the primary or the secondary Connected Sources client authentication key   
        static string sharedKey = "+nF2QZkgZ6EEId3wa/IoYvMGT/zkKcElSlgmWsaHINpdx9Dv0jUkTAg6/724uH2pxpkq8Bd7ILt3QLoEKUvbHA==";

        // LogName is name of the event type that is being submitted to Log Analytics
        static string LogName = "MineCraftDemoDataNew";

        // You can use an optional field to specify the timestamp from the data. If the time field is not specified, Log Analytics assumes the time is the message ingestion time
        static string TimeStampField = "";
        // Create a hash for the API signature

        static void Main(string[] args)
        {
            // An example JSON object, with key/value pairs
            //static string json = @"[{""DemoField1"":""DemoValue1"",""DemoField2"":""DemoValue2""},{""DemoField3"":""DemoValue3"",""DemoField4"":""DemoValue4""}]";
            //var json = GetJsonData();
            var json = @"[{""MaxPlayers"":""600"",""CurrentPlayers"":""9""}]";


            
            Console.WriteLine(json);
            // Update customerId to your Log Analytics workspace ID

            var datestring = DateTime.UtcNow.ToString("r");
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
            string hashedString = BuildSignature(stringToHash, sharedKey);
            string signature = "SharedKey " + customerId + ":" + hashedString;

            PostData(signature, datestring, json);

            Console.ReadKey();
        }

        public static string BuildSignature(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GetJsonData()
        {
            var url = "https://mcapi.us/server/status?ip=52.147.12.155.nerd.nu";

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            System.Net.Http.HttpContent httpContent = new StringContent("", Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Task<System.Net.Http.HttpResponseMessage> response = client.GetAsync(url);

            System.Net.Http.HttpContent responseContent = response.Result.Content;
            
            string result = responseContent.ReadAsStringAsync().Result;
            Console.WriteLine("Return Result: " + result);
            return result;

        }

        // Send a request to the POST API endpoint
        public static void PostData(string signature, string date, string json)
        {
            try
            {
                string url = "https://" + customerId + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Log-Type", LogName);
                client.DefaultRequestHeaders.Add("Authorization", signature);
                client.DefaultRequestHeaders.Add("x-ms-date", date);
                
                System.Net.Http.HttpContent httpContent = new StringContent(json, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                Task<System.Net.Http.HttpResponseMessage> response = client.PostAsync(new Uri(url), httpContent);

                System.Net.Http.HttpContent responseContent = response.Result.Content;
                string result = responseContent.ReadAsStringAsync().Result;
            }
            catch (Exception excep)
            {
                Console.WriteLine("API Post Exception: " + excep.Message);
            }
        }
    }
}
