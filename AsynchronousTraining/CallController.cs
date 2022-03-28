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
        private readonly List<(IHttpCallable caller, int concurrentRequestLimit)> CallerList = new List<(IHttpCallable, int)>();

        private readonly BlockingCollection<IHttpCallable> IdleCallers = new BlockingCollection<IHttpCallable>();

        private readonly int ConcurrentRequestLimit;


        public CallController(int concurrentRequestLimit)
        {
            ConcurrentRequestLimit = concurrentRequestLimit;
        }

        /// <summary>
        /// 呼叫器數量
        /// </summary>
        public int Count
        {
            get => CallerList.Count;
        }

        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        public int AddCaller(IHttpCallable caller)
        {
            IdleCallers.Add(caller);
            return Count - 1;
        }

        /// <summary>
        /// 隨機選擇一個呼叫器，呼叫API
        /// </summary>
        /// <param name="request">請求</param>
        /// <returns>回覆</returns>
        public async Task<Response> PostAsync(Request request)
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("CallerList empty");
            }

            if (IdleCallers.Count == CallerList.Count)
            {
                return await RandomAssignCall(request);
            }

            return await RandomAssignCall(request);
        }

        private async Task<Response> RandomAssignCall(Request request)
        {
            int random = Random.Next(0, Count);
            Console.WriteLine("Using Caller " + random);

            IHttpCallable caller = CallerList[random].caller;
            
            Response response = await caller.PostAsync(request);
            return response;
        }
    }

}
