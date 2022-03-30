using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace AsynchronousTraining
{
    public class CallConsumer
    {
        private readonly ConcurrentQueue<Request> IdleRequests;

        private readonly int Countdown;

        private readonly IHttpCallable Caller;

        private readonly SemaphoreSlim Semaphore;

        public CallConsumer(IHttpCallable caller, ConcurrentQueue<Request> idleRequests, int requestLimit)
        {
            IdleRequests = idleRequests;
            Caller = caller;
            Semaphore = new SemaphoreSlim(requestLimit);
        }

        public void Run()
        {
            while (true)
            {
                Semaphore.Wait();
                IdleRequests.TryDequeue(out var request);
                Caller.PostAsync(request);
            }
        }
    }
}
