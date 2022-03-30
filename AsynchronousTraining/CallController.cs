using System.Collections.Concurrent;
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
        /// 閒置呼叫器呼叫額度佇列
        /// </summary>
        private readonly ConcurrentQueue<IHttpCallable> IdleRequestQuotas = new ConcurrentQueue<IHttpCallable>();

        /// <summary>
        /// 號誌
        /// </summary>
        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(0);

        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        /// <param name="requestLimit">呼叫器的呼叫次數限制</param>
        public void AddCaller(IHttpCallable caller, int requestLimit)
        {
            for (int i=0; i<requestLimit; i++)
            {
                IdleRequestQuotas.Enqueue(caller);
            }

            Semaphore.Release(requestLimit);
        }

        /// <summary>
        /// 使用IdleCallers中閒置的呼叫器呼叫API
        /// </summary>
        /// <param name="request">請求</param>
        /// <returns>回覆</returns>
        public async Task<Response> PostAsync(Request request)
        {
            await Semaphore.WaitAsync();

            // 取出caller
            IdleRequestQuotas.TryDequeue(out var caller);
            var response = await caller.PostAsync(request);
            IdleRequestQuotas.Enqueue(caller);

            Semaphore.Release();
            return response;
        }
    }
}