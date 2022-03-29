using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousTraining
{
    public class CallController : IHttpCallable
    {
        /// <summary>
        /// 隨機
        /// </summary>
        private readonly Random Random = new Random();

        /// <summary>
        /// 呼叫器列表
        /// </summary>
        //private readonly List<(IHttpCallable caller, int concurrentRequestLimit)> CallerList = new List<(IHttpCallable, int)>();

        private readonly BlockingCollection<(int, IHttpCallable, int)> IdleCallers;

        private readonly ConcurrentDictionary<int, int> ConcurrentRequestCountTracker;

        private int CallerNum = 0;


        public CallController()
        {
            IdleCallers = new BlockingCollection<(int id, IHttpCallable caller, int concurrentRequestLimit)>();
            ConcurrentRequestCountTracker = new ConcurrentDictionary<int, int>();
        }

        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        public void AddCaller(IHttpCallable caller, int concurrentRequestLimit)
        {
            int concurrentRequestCount = 0;
            IdleCallers.Add((CallerNum, caller, concurrentRequestLimit));
            ConcurrentRequestCountTracker[CallerNum] = concurrentRequestCount;
            CallerNum++;
        }

        /// <summary>
        /// 隨機選擇一個呼叫器，呼叫API
        /// </summary>
        /// <param name="request">請求</param>
        /// <returns>回覆</returns>
        public async Task<Response> PostAsync(Request request)
        {
            Response response;

            var (id, caller, concurrentRequestLimit) = IdleCallers.Take();
            if (ConcurrentRequestCountTracker.AddOrUpdate(id, 0, (key, oldValue) => oldValue + 1) < concurrentRequestLimit)
            {
                Console.WriteLine("Requested with caller: " + id);

                IdleCallers.Add((id, caller, concurrentRequestLimit));
                response = await caller.PostAsync(request);
                ConcurrentRequestCountTracker.AddOrUpdate(id, 0, (key, oldValue) => oldValue - 1);

                Console.WriteLine("Got response from caller: " + id);
            }
            else if (ConcurrentRequestCountTracker[id] == concurrentRequestLimit)
            {
                Console.WriteLine("[Max Limit] Requested with caller: " + id);

                // Add back to IdleCallers queue after getting response, stops this caller from getting used until await finishes
                response = await caller.PostAsync(request);
                IdleCallers.Add((id, caller, concurrentRequestLimit));
                ConcurrentRequestCountTracker.AddOrUpdate(id, 0, (key, oldValue) => oldValue - 1);

                Console.WriteLine("[Max Limit] Got response from caller: " + id);
            }
            else
            {
                // TODO: not sure what to return
                Console.WriteLine("response null");
                response = null;
            }

            return response;
        }
    }

}