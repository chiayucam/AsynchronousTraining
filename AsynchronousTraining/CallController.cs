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

        private readonly BlockingCollection<(int, IHttpCallable)> IdleCallers;

        private int CallerNum = 0;


        public CallController()
        {
            IdleCallers = new BlockingCollection<(int id, IHttpCallable caller)>();
        }

        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        public void AddCaller(IHttpCallable caller)
        {
            IdleCallers.Add((CallerNum, caller));
            CallerNum++;
        }

        /// <summary>
        /// 隨機選擇一個呼叫器，呼叫API
        /// </summary>
        /// <param name="request">請求</param>
        /// <returns>回覆</returns>
        public async Task<Response> PostAsync(Request request)
        {
            if (IdleCallers.Count == CallerNum)
            {
                return await RandomAssignCall(request);
            }


            var (id, caller) = IdleCallers.Take();

            Response response;
            Console.WriteLine("Requested with caller: " + id);

            if 
            try
            {
                response = await caller.PostAsync(request);
            }
            catch (RequestLimitExceededException)
            {
                
            }
            Console.WriteLine("Got response from caller: " + id);

            IdleCallers.Add((id, caller));

            return response;
        }

        private async Task<Response> RandomAssignCall(Request request)
        {
            int random = Random.Next(0, IdleCallers.Count);

            int id;
            IHttpCallable caller;

            for (int i=0; i<random; i++)
            {
                IdleCallers.Add(IdleCallers.Take());
            }

            (id, caller) = IdleCallers.Take();

            //while(true)
            //{
            //    (id, caller) = IdleCallers.Take();
            //    if (id == random) break;
            //    IdleCallers.Add((id, caller));
            //}

            Console.WriteLine("[Random] Requested with caller: " + id);
            Response response = await caller.PostAsync(request);
            Console.WriteLine("[Random] Got response from caller: " + id);
            IdleCallers.Add((id, caller));

            return response;
        }
    }

}