using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousTraining
{
    /// <summary>
    /// API呼叫分配控制器
    /// </summary>
    public class CallController : IHttpCallable
    {
        /// <summary>
        /// 閒置呼叫器佇列
        /// </summary>
        private readonly BlockingCollection<(int id, IHttpCallable caller, int concurrentRequestLimit)> IdleCallers;

        private readonly BlockingCollection<Request> IdleRequests;

        /// <summary>
        /// 呼叫次數追蹤器
        /// </summary>
        private readonly ConcurrentDictionary<int, int> ConcurrentRequestCountTracker;

        /// <summary>
        /// 建構子
        /// </summary>
        public CallController(int requestLimit)
        {
            IdleCallers = new BlockingCollection<(int id, IHttpCallable caller, int concurrentRequestLimit)>();
            ConcurrentRequestCountTracker = new ConcurrentDictionary<int, int>();
            IdleRequests = new BlockingCollection<Request>(requestLimit);
        }

        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        /// <param name="concurrentRequestLimit">呼叫器的呼叫次數限制</param>
        public void AddCaller(IHttpCallable caller, int concurrentRequestLimit)
        {
            int callerId = IdleCallers.Count;
            int concurrentRequestCount = 0;
            IdleCallers.Add((callerId, caller, concurrentRequestLimit));
            ConcurrentRequestCountTracker[callerId] = concurrentRequestCount;
        }

        /// <summary>
        /// 使用IdleCallers中閒置的呼叫器呼叫API
        /// </summary>
        /// <param name="request">請求</param>
        /// <returns>回覆</returns>
        public async Task<Response> PostAsync(Request request)
        {
            //Response response;

            //// 取出caller
            //var (id, caller, concurrentRequestLimit) = IdleCallers.Take();

            //// 呼叫次數 + 1，如果此數小於呼叫數量限制執行if，如果此數等於呼叫數量限制執行elseif
            //if (ConcurrentRequestCountTracker.AddOrUpdate(id, 0, (key, oldValue) => oldValue + 1) < concurrentRequestLimit)
            //{
            //    Console.WriteLine("Requested with caller: " + id);

            //    // 呼叫器加回IdleCallers，使它可以再次被取用
            //    IdleCallers.Add((id, caller, concurrentRequestLimit));
            //    response = await caller.PostAsync(request);

            //    // reqeust完成後呼叫次數 - 1
            //    ConcurrentRequestCountTracker.AddOrUpdate(id, 0, (key, oldValue) => oldValue - 1);

            //    Console.WriteLine("Got response from caller: " + id);
            //}
            //else if (ConcurrentRequestCountTracker[id] == concurrentRequestLimit)
            //{
            //    Console.WriteLine("[Max Limit] Requested with caller: " + id);

            //    // request完成後再將呼叫器加回IdleCallers
            //    response = await caller.PostAsync(request);
            //    IdleCallers.Add((id, caller, concurrentRequestLimit));

            //    // 呼叫次數 - 1
            //    ConcurrentRequestCountTracker.AddOrUpdate(id, 0, (key, oldValue) => oldValue - 1);

            //    Console.WriteLine("[Max Limit] Got response from caller: " + id);
            //}
            //else
            //{
            //    response = null;
            //}

            //return response;

            IdleRequests.Add(request);
        }
    }
}