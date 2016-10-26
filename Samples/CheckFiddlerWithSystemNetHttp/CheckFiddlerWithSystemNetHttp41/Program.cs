using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;

namespace CheckFiddlerWithSystemNetHttp41
{
    //from https://www.dotnetperls.com/httpclient
    class Program
    {
        static void Main()
        {
            var task=ExampleFlurl();
            Console.WriteLine("Downloading page(Flurl)...");
            Console.WriteLine(task.Result.Substring(0, 50) + "...");
            ExampleFromDotPerl();
        }

        private static async Task<string> ExampleFlurl()
        {
            var getResp = await "http://en.wikipedia.org/".GetStringAsync();
            return getResp;
        }

        static void ExampleFromDotPerl()
        {
            Task t = new Task(DownloadPageAsync);
            t.Start();
            Console.WriteLine("Downloading page(FromDotPerl)...");
            Console.ReadLine();
        }
        static async void DownloadPageAsync()
        {
            // ... Target page.
            string page = "http://en.wikipedia.org/";

            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                if (result != null &&
                result.Length >= 50)
                {
                    Console.WriteLine(result.Substring(0, 50) + "...");
                }
            }
        }
    }

}
