using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousTraining
{
    public class ConcurrentRequestLimitDecorator : IHttpCallable
    {
        private readonly IHttpCallable Caller;

        private readonly int ConcurrentRequestLimit;

        private int ConcurrentRequestCount;

        public int Count
        {
            get => ConcurrentRequestCount;
        }

        public ConcurrentRequestLimitDecorator(IHttpCallable caller, int concurrentRequestLimit)
        {
            Caller = caller;
            ConcurrentRequestLimit = concurrentRequestLimit;
        }

        public async Task<Response> PostAsync(Request request)
        {
            if (Interlocked.Increment(ref ConcurrentRequestCount) <= ConcurrentRequestLimit)
            {
                var response = await Caller.PostAsync(request);
                Interlocked.Decrement(ref ConcurrentRequestCount);
                return response;
            }
            else
            {
                Interlocked.Decrement(ref ConcurrentRequestCount);
                throw new RequestLimitExceededException("Maximun concurrent request limit exceeded.");
            }
        }

        public int Increment()
        {
            return Interlocked.Increment(ref ConcurrentRequestCount);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref ConcurrentRequestCount);
        }
    }
}
