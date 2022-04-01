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
    public class CallProducerConsumerController
    {
        /// <summary>
        /// 生產者 生產Request
        /// </summary>
        private readonly CallProducer CallProducer;

        /// <summary>
        /// 消費者 消費Request
        /// </summary>
        private readonly List<CallConsumer> CallConsumers = new List<CallConsumer>();

        /// <summary>
        /// Request 通道，提供給生產者存放，消費者取用
        /// </summary>
        private readonly Channel<Request> RequestChannel = Channel.CreateUnbounded<Request>();

        /// <summary>
        /// Response
        /// </summary>
        private readonly Channel<Response> ResponseChannel = Channel.CreateUnbounded<Response>();

        public CallProducerConsumerController()
        {
            CallProducer = new CallProducer(RequestChannel.Writer);
        }

        public List<Response> Responses = new List<Response>();

        /// <summary>
        /// 加呼叫器
        /// </summary>
        /// <param name="caller">呼叫器</param>
        /// <param name="requestLimit">呼叫器的呼叫次數限制</param>
        public void AddCallConsumer(IHttpCallable caller, int requestLimit)
        {
            CallConsumers.Add(new CallConsumer(caller, RequestChannel.Reader, ResponseChannel.Writer, requestLimit));
        }

        public void AddRequest(Request request)
        {
            CallProducer.AddRequest(request);
        }

        public void ProducerComplete()
        {
            CallProducer.Complete();
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

            await Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        Responses.Add(await ResponseChannel.Reader.ReadAsync());
                    }
                }
                catch (ChannelClosedException)
                {
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
