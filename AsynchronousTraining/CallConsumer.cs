using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace AsynchronousTraining
{
    public class CallConsumer
    {
        private readonly IHttpCallable Caller;

        private readonly ChannelReader<Request> ChannelReader;

        public CallConsumer(IHttpCallable caller, ChannelReader<Request> channel, int requestLimit)
        {
            Caller = caller;
            ChannelReader = channel;
            RequestLimit = requestLimit;
        }

        public int RequestLimit { get; }


        public async Task<Response> StartConsumeAsync()
        {
            // TODO: how to return Task<Response>
            while (true)
            {
                var request = await ChannelReader.ReadAsync();
                var response = await Caller.PostAsync(request);
                Console.WriteLine($"[Response] {response.IsCompleted}");
            }
        }
    }
}
