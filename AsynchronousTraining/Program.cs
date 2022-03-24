using System;
using System.Collections.Generic;
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
        private static async Task Main()
        {
            // get configurations from appsettings.json
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var config = builder.Build();

            // get baseUri for api
            string customReportbaseUri = config.GetSection("CustomReport")["baseUri"];

            // testing parameters
            Request request = new Request
            {
                Dtno = 5493,
                Ftno = 0,
                @Params = "AssignID=00878;AssignDate=20200101-99999999;DTOrder=2;MTPeriod=2;",
                KeyMap = "",
                AssignSpid = ""
            };

            var customReportCaller = new CustomReportCaller(customReportbaseUri, Client);
            var response = await customReportCaller.PostAsync(request);

            // TODO: check if response is correct, remove later
            //PrintResponse(response);


            // mock caller
            int responseTime = 1000;
            int maxConcurrentRequest = 5;

            var customReportMockCaller = new CustomReportMockCaller(responseTime, maxConcurrentRequest);

            // make multiple calls
            var tasks = new List<Task>();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(customReportMockCaller.PostAsync(request));
                }

                await Task.WhenAll(tasks);
            }
            catch (RequestLimitExceededException)
            {
                Console.WriteLine("Request Limit Exceeded");
            }

            Console.ReadLine();
        }

        /// <summary>
        /// 印出Response
        /// </summary>
        /// <param name="response">response</param>
        private static void PrintResponse(Response response)
        {
            Console.WriteLine(response.IsCompleted);
            Console.WriteLine(response.IsFaulted);
            Console.WriteLine(response.Signature);
            Console.WriteLine(response.Expception);
            Console.WriteLine(response.Result);
        }
    }
}
