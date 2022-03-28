using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousTraining
{
    /// <summary>
    /// 呼叫分配器
    /// </summary>
    public class CallRandomAllocator : IHttpCallable
    {
        /// <summary>
        /// 隨機
        /// </summary>
        private readonly Random Random = new Random();

        /// <summary>
        /// 呼叫器列表
        /// </summary>
        private readonly List<IHttpCallable> CallerList = new List<IHttpCallable>();

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
            CallerList.Add(caller);
            return Count - 1;
        }

        /// <summary>
        /// 替換呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        /// <param name="callerId">呼叫器Id</param>
        public void ReplaceCaller(IHttpCallable caller, int callerId)
        {
            CallerList[callerId] = caller;
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

            int random = Random.Next(0, Count);
            Console.WriteLine("Using Caller " + random);
            Response response = await CallerList[random].PostAsync(request);
            return response;
        }
    }
}
