using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Text;

namespace AsynchronousTraining
{
    public class CallProducerConsumerController
    {
        private readonly List<CallConsumer> CallConsumers = new List<CallConsumer>();

        private Channel<Request> RequestChannel = Channel.CreateUnbounded<Request>();

        private ConcurrentQueue<Request> IdleRequests = new ConcurrentQueue<Request>();


        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        /// <param name="requestLimit">呼叫器的呼叫次數限制</param>
        public void AddCallConsumer(IHttpCallable caller, Channel<Request> channel, int requestLimit)
        {
            CallConsumers.Add(new CallConsumer(caller, channel, requestLimit));
        }

        public async Task StartConsumeAsync()
        {
            var tasks = new List<Task>();
            foreach (var callConsumer in CallConsumers)
            {
                for (int i = 0; i < callConsumer.RequestLimit; i++)
                {
                    tasks.Add(callConsumer.StartConsumeAsync());
                }
            }

            await Task.WhenAll(tasks);
        }

    }
}
