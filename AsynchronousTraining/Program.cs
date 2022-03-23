using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AsynchronousTraining
{
    /// <summary>
    /// 主程式進入點
    /// </summary>
    public class Program
    {
        private static HttpClient Client = new HttpClient();

        /// <summary>
        /// 主程式
        /// </summary>
        /// <param name="args">參數</param>
        /// <returns>Task</returns>
        private static async Task Main(string[] args)
        {
            // get configurations from appsettings.json
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var config = builder.Build();

            // get baseUri for api
            string customReportbaseUri = config.GetSection("CustomReport")["baseUri"];

            // testing parameters
            CustomReportRequest request = new CustomReportRequest
            {
                Dtno = 5493,
                Ftno = 0,
                @Params = "AssignID=00878;AssignDate=20200101-99999999;DTOrder=2;MTPeriod=2;",
                KeyMap = "",
                AssignSpid = ""
            };

            var customReportCaller = new CustomReportCaller(customReportbaseUri, Client);
            var response = await customReportCaller.PostAsync(request);

            // TODO: check response correct, remove later
            Console.WriteLine(response.IsCompleted);
            Console.WriteLine(response.IsFaulted);
            Console.WriteLine(response.Signature);
            Console.WriteLine(response.Expception);
            Console.WriteLine(response.Result);


            

            CustomReportMockCaller.MaxConcurrentRequest = 50;
            Console.WriteLine(CustomReportMockCaller.CurrentConcurrentRequest);

            for (int i=0; i<100; i++)
            {
                var customReportMockCaller = new CustomReportMockCaller(customReportbaseUri, Client, 1000);
                await customReportMockCaller.PostAsync(request);
            }
        }
    }
}
