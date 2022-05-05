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
        private readonly ChannelWriter<(Request, TaskCompletionSource<Response>)> RequestWriter;

        public CallProducer(ChannelWriter<(Request, TaskCompletionSource<Response>)> requestWriter)
        {
            RequestWriter = requestWriter;
        }

        public void AddRequest(Request request, TaskCompletionSource<Response> taskCompletionSource)
        {
            RequestWriter.WriteAsync((request, taskCompletionSource));
        }
    }
}
