using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AsynchronousTraining
{
    public class CustomReportMockCaller : ICustomReportCaller
    {
        private static object locker = new object();

        private int ResponseTime;

        private int ConcurrentRequestLimit;

        private int ConcurrentRequest;

        public CustomReportMockCaller(int responseTime, int maxConcurrentRequest)
        {
            ResponseTime = responseTime;
            ConcurrentRequestLimit = maxConcurrentRequest;
        }

        /// <summary>
        /// Mock呼叫API
        /// </summary>
        /// <param name="request">Request類別的request body</param>
        /// <returns>Response類別的response body</returns>
        public async Task<Response> PostAsync(Request request)
        {
            if (ConcurrentRequest < ConcurrentRequestLimit)
            {
                lock (locker)
                {
                    ConcurrentRequest++;
                    LogRequest();
                }

                await Task.Delay(ResponseTime);

                lock (locker)
                {
                    ConcurrentRequest--;
                    LogReponse();
                }
            }
            else
            {
                throw new RequestLimitExceededException("MaxConcurrentRequest: " + ConcurrentRequestLimit);
            }

            var response = new Response();
            return response;
        }

        /// <summary>
        /// log request
        /// </summary>
        private void LogRequest()
        {
            Console.WriteLine("Request:  " + GetTimestamp(DateTime.Now) + " CurrentConcurrentRequests: " + ConcurrentRequest);
        }

        /// <summary>
        /// log response
        /// </summary>
        private void LogReponse()
        {
            Console.WriteLine("Response: " + GetTimestamp(DateTime.Now) + " CurrentConcurrentRequests: " + ConcurrentRequest);
        }

        /// <summary>
        /// 按格式轉換Datetime成string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetTimestamp(DateTime value)
        {
            return value.ToString("HH:mm:ss:ffff");
        }

    }
}
