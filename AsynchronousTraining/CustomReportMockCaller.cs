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
            if (Interlocked.Increment(ref ConcurrentRequestCount) <= ConcurrentRequestLimit)
            {
                await Task.Delay(ResponseTime);

                Interlocked.Decrement(ref ConcurrentRequestCount);
            }
            else
            {
                Interlocked.Decrement(ref ConcurrentRequestCount);
                throw new RequestLimitExceededException("Maximun concurrent request limit exceeded.");
            }

            var response = new Response()
            {
                Result = request.Params
            };
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
    }
}
