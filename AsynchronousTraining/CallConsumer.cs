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

        private readonly ChannelReader<Request> RequestReader;

        private readonly ChannelWriter<Response> ResponseWriter;

        public CallConsumer(IHttpCallable caller, ChannelReader<Request> requestReader, ChannelWriter<Response> responseWriter, int requestLimit)
        {
            Caller = caller;
            RequestReader = requestReader;
            ResponseWriter = responseWriter;
            RequestLimit = requestLimit;
        }

        public int RequestLimit { get; }


        public async Task StartConsumeAsync()
        {
            // TODO: how to return Task<Response>
            try
            {
                while (true)
                {
                    var request = await RequestReader.ReadAsync();

                    Console.WriteLine($"[Request] {request}");
                    var response = await Caller.PostAsync(request);
                    Console.WriteLine($"[Response] {response.IsCompleted}");

                    await ResponseWriter.WriteAsync(response);
                }
            }
            catch (ChannelClosedException)
            {
                Console.WriteLine("Channel job completed");

                // multiple threads trying to close Response channel
                try 
                {
                    ResponseWriter.Complete();
                }
                catch (ChannelClosedException)
                {
                }
            }
        }
    }
}
