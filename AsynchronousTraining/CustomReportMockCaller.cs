using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AsynchronousTraining
{
    /// <summary>
    /// 自訂報表假物件
    /// </summary>
    public class CustomReportMockCaller : IHttpCallable
    {
        private readonly int ResponseTime;

        private readonly int ConcurrentRequestLimit;

        private int ConcurrentRequestCount;

        public CustomReportMockCaller(int responseTime, int concurrentRequestLimit)
        {
            ResponseTime = responseTime;
            ConcurrentRequestLimit = concurrentRequestLimit;
        }

        /// <summary>
        /// Mock呼叫API
        /// </summary>
        /// <param name="request">Request類別的request body</param>
        /// <returns>Response類別的response body</returns>
        public async Task<Response> PostAsync(Request request)
        {
            // atomic operation increment ConcurrentRequestCount, then check if count is under limit
            //if (InterlockedIncrementIfLessThan(ref ConcurrentRequestCount, ConcurrentRequestLimit))
            if (Interlocked.Increment(ref ConcurrentRequestCount) <= ConcurrentRequestLimit)
            {
                //LogRequest();

                await Task.Delay(ResponseTime);

                Interlocked.Decrement(ref ConcurrentRequestCount);
                //LogReponse();
            }
            else
            {
                Interlocked.Decrement(ref ConcurrentRequestCount);
                //LogRequestLimitExceeded();
                throw new RequestLimitExceededException("Maximun concurrent request limit exceeded.");
            }

            var response = new Response();
            return response;
        }

        private bool InterlockedIncrementIfLessThan(ref int location, int comparand)
        {
            int initialValue;
            int newValue;
            do
            {
                initialValue = location;
                newValue = initialValue + 1;
                if (initialValue >= comparand) return false;
            }
            while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);
            return true;
        }

        /// <summary>
        /// log request
        /// </summary>
        private void LogRequest()
        {
            Console.WriteLine("Request:  " + GetTimestamp(DateTime.Now) + " CurrentConcurrentRequests: " + ConcurrentRequestCount);
        }

        /// <summary>
        /// log response
        /// </summary>
        private void LogReponse()
        {
            Console.WriteLine("Response: " + GetTimestamp(DateTime.Now) + " CurrentConcurrentRequests: " + ConcurrentRequestCount);
        }

        /// <summary>
        /// log request limit exceeded
        /// </summary>
        private void LogRequestLimitExceeded()
        {
            Console.WriteLine("Request Limit Exceeded");
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
