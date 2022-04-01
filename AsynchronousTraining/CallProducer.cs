using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Text;

namespace AsynchronousTraining
{
    public class CallProducer
    {
        private readonly ChannelWriter<Request> ChannelWriter;

        public CallProducer(ChannelWriter<Request> channel)
        {
            ChannelWriter = channel;
        }

        public void AddRequest(Request request)
        {
            //IdleRequests.Enqueue(request);
            ChannelWriter.WriteAsync(request);
        }

        public void Complete()
        {
            ChannelWriter.Complete();
        }
    }
}
