using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace AsynchronousTraining
{
    public class CallProducer
    {
        /// <summary>
        /// 閒置呼叫器呼叫額度佇列
        /// </summary>
        private readonly ConcurrentQueue<Request> IdleRequests = new ConcurrentQueue<Request>();

        /// <summary>
        /// 呼叫器列表
        /// </summary>
        private readonly List<(IHttpCallable caller, int requestLimit)> CallerList = new List<(IHttpCallable caller, int requestLimit)>();

        /// <summary>
        /// 號誌
        /// </summary>
        private SemaphoreSlim Semaphore = new SemaphoreSlim(0);

        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        /// <param name="requestLimit">呼叫器的呼叫次數限制</param>
        public void AddCaller(IHttpCallable caller, int requestLimit)
        {
            CallerList.Add((caller, requestLimit));
        }

        public void AddRequest(Request request)
        {
            IdleRequests.Enqueue(request);
        }

        public async Task<Response> PostAsync(Request request)
        {
            IdleRequests.Enqueue(request);
            var response = await Consume(request);
            return response;
        }

        public Task<Response> Consume(Request request)
        {
            while (true)
        }
    }
}
