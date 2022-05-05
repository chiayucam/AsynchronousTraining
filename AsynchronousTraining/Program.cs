using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AsynchronousTraining
{
    /// <summary>
    /// 主程式進入點
    /// </summary>
    public class Program
    {
        /// <summary>
        /// HttpClient
        /// </summary>
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

            //await TaskOne(customReportbaseUri, request);

            //await TaskTwo(request);

            //await TaskThree(customReportbaseUri, request);

            //await TaskFour(customReportbaseUri, request);

            TaskFive(customReportbaseUri, request);

            Console.ReadLine();
        }

        private static async Task TaskOne(string customReportbaseUri, Request request)
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

        private static async Task TaskTwo(Request request)
        {
            // mock caller
            int responseTime = 1000;
            int concurrentRequestLimit = 5;

            var customReportMockCaller = new CustomReportMockCaller(responseTime, concurrentRequestLimit);

            // make multiple calls
            var tasks = new List<Task>();
            try
            {
                for (int i=0; i<10; i++)
                {
                    tasks.Add(customReportMockCaller.PostAsync(request));
                }

                await Task.WhenAll(tasks);
            }
            catch (RequestLimitExceededException)
            {
            }
        }

        private static async Task TaskThree(string customReportbaseUri, Request request)
        {
            // mock caller arguments
            int responseTime = 1000;
            int concurrentRequestLimit = 5;

            CallRandomAllocator callRandomAllocator = new CallRandomAllocator();

            // add callers
            for (int i=0; i<3; i++)
            {
                callRandomAllocator.AddCaller(new CustomReportCaller(customReportbaseUri, Client));
            }

            var tasks = new List<Task>();
            try
            {
                for (int i=0; i<100; i++)
                {
                    tasks.Add(callRandomAllocator.PostAsync(request));
                }

                await Task.WhenAll(tasks);
            }
            catch (RequestLimitExceededException)
            {
            }

            Console.ReadLine();

            //replace last caller with mock caller
            callRandomAllocator.ReplaceCaller(new CustomReportMockCaller(responseTime, concurrentRequestLimit), callRandomAllocator.Count - 1);

            tasks.Clear();
            try
            {
                for (int i=0; i<100; i++)
                {
                    tasks.Add(callRandomAllocator.PostAsync(request));
                }

                await Task.WhenAll(tasks);
            }
            catch (RequestLimitExceededException)
            {
            }
        }

        private static async Task TaskFour(string customReportbaseUri, Request request)
        {
            CallController callController = new CallController();

            // add callers
            for (int i=0; i<3; i++)
            {
                int concurrentRequestLimit = 3;
                //callController.AddCaller(new ConcurrentRequestLimitDecorator(new CustomReportCaller(customReportbaseUri, Client), concurrentRequestLimit));
                callController.AddCaller(new CustomReportMockCaller((i + 1) * 1000, 10), concurrentRequestLimit);
            }

            var tasks = new List<Task>();
            try
            {
                for (int i = 0; i < 30; i++)
                {
                    tasks.Add(callController.PostAsync(request));
                }

                await Task.WhenAll(tasks);
            }
            catch (RequestLimitExceededException)
            {
            }
        }

        private static void TaskFive(string customReportbaseUri, Request request)
        {
            // testing parameters
            int requestLimit = 10;

            // mock caller parameters
            int responseTime = 10;
            int mockCallerRequestLimit = 3;

            // create callconsumers
            var callconsumers = new CallConsumer[]
            {
                //new CallConsumer(new CustomReportCaller(customReportbaseUri, Client), requestLimit),
                //new CallConsumer(new CustomReportCaller(customReportbaseUri, Client), requestLimit)
                new CallConsumer(new CustomReportMockCaller(responseTime, mockCallerRequestLimit), requestLimit),
                new CallConsumer(new CustomReportMockCaller(responseTime, mockCallerRequestLimit), requestLimit)
            };

            var callProducerConsumerController = new CallProducerConsumerController(callconsumers);

            foreach (var index in Enumerable.Range(0, 100))
            {
                Task.Run(async () =>
                {
                    var request = new Request()
                    {
                        Params = index.ToString()
                    };
                    var response = await callProducerConsumerController.PostAsync(request);
                    Console.WriteLine($"request={request.Params},response={response.Result}");
                });
            }
        }
    }
}
