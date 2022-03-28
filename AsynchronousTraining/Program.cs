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

            // testing request parameters
            Request request = new Request
            {
                Dtno = 5493,
                Ftno = 0,
                @Params = "AssignID=00878;AssignDate=20200101-99999999;DTOrder=2;MTPeriod=2;",
                KeyMap = "",
                AssignSpid = ""
            };

            //await TestOne(customReportbaseUri, request);

            //await TestTwo(request);

            //await TestThree(customReportbaseUri, request);

            await TestFour(customReportbaseUri, request);

            Console.ReadLine();
        }

        private static async Task TestOne(string customReportbaseUri, Request request)
        {
            

            var customReportCaller = new CustomReportCaller(customReportbaseUri, Client);
            var response = await customReportCaller.PostAsync(request);

            // check if response is correct
            Console.WriteLine(response.IsCompleted);
            Console.WriteLine(response.IsFaulted);
            Console.WriteLine(response.Signature);
            Console.WriteLine(response.Expception);
            Console.WriteLine(response.Result);
        }

        private static async Task TestTwo(Request request)
        {
            // mock caller
            int responseTime = 1000;
            int concurrentRequestLimit = 5;

            var customReportMockCaller = new CustomReportMockCaller(responseTime, concurrentRequestLimit);

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
            }
        }

        private static async Task TestThree(string customReportbaseUri, Request request)
        {
            // mock caller arguments
            int responseTime = 1000;
            int concurrentRequestLimit = 5;

            CallRandomAllocator callAllocator = new CallRandomAllocator();

            // add callers
            for (int i=0; i<3; i++)
            {
                callAllocator.AddCaller(new CustomReportCaller(customReportbaseUri, Client));
            }

            var tasks = new List<Task>();
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    tasks.Add(callAllocator.PostAsync(request));
                }

                await Task.WhenAll(tasks);
            }
            catch (RequestLimitExceededException)
            {
            }

            Console.ReadLine();

            //replace last caller with mock caller
            callAllocator.ReplaceCaller(new CustomReportMockCaller(responseTime, concurrentRequestLimit), callAllocator.Count - 1);

            tasks.Clear();
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    tasks.Add(callAllocator.PostAsync(request));
                }

                await Task.WhenAll(tasks);
            }
            catch (RequestLimitExceededException)
            {
            }
        }

        private static async Task TestFour(string customReportbaseUri, Request request)
        {
            // mock caller arguments
            int responseTime = 1000;
            int concurrentRequestLimit = 5;

            CallController callController = new CallController(concurrentRequestLimit);


        }
    }
}
