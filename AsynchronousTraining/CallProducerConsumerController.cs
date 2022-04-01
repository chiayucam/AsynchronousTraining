using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Text;

namespace AsynchronousTraining
{
    /// <summary>
    /// 生產者/消費者模式 呼叫API控制器
    /// </summary>
    public class CallProducerConsumerController : IHttpCallable
    {
        /// <summary>
        /// 生產者 生產Request
        /// </summary>
        private readonly CallProducer CallProducer;

        /// <summary>
        /// 消費者陣列 消費Request
        /// </summary>
        private readonly CallConsumer[] CallConsumers;

        /// <summary>
        /// Request 通道，提供給生產者存放，消費者取用
        /// </summary>
        private readonly Channel<(Request, TaskCompletionSource<Response>)> RequestChannel = 
            Channel.CreateUnbounded<(Request, TaskCompletionSource<Response>)>();

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="callConsumers">消費者陣列</param>
        public CallProducerConsumerController(CallConsumer[] callConsumers)
        {
            CallConsumers = callConsumers;
            foreach (var callConsumer in CallConsumers)
            {
                callConsumer.RequestReader = RequestChannel.Reader;
            }

            CallProducer = new CallProducer(RequestChannel.Writer);
        }

        /// <summary>
        /// 關閉 RequestChannel
        /// </summary>
        public void ProducerComplete()
        {
            CallProducer.Complete();
        }

        /// <summary>
        /// 啟動Consumers
        /// </summary>
        public void StartConsumers()
        {
            // spawn RequestLimit amount of threads for each callConsumer
            foreach (var callConsumer in CallConsumers)
            {
                for (int i = 0; i < callConsumer.RequestLimit; i++)
                {
                    Task.Run(() => callConsumer.StartConsumeAsync());
                }
            }
        }

        /// <summary>
        /// 傳入request 呼叫API
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public async Task<Response> PostAsync(Request request)
        {
            var taskCompletionSource = new TaskCompletionSource<Response>();

            CallProducer.AddRequest(request, taskCompletionSource);

            return await taskCompletionSource.Task;
        }
    }
}
