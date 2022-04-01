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

        //private readonly ChannelReader<Request> RequestReader;

        //private readonly ChannelWriter<Response> ResponseWriter;

        public CallConsumer(IHttpCallable caller, int requestLimit)
        {
            Caller = caller;
            //RequestReader = requestReader;
            //ResponseWriter = responseWriter;
            RequestLimit = requestLimit;
        }

        public ChannelReader<(Request, TaskCompletionSource<Response>)> RequestReader { get; set; }

        //public ChannelWriter<Response> ResponseWriter { get; set; }

        public int RequestLimit { get; }


        public async Task StartConsumeAsync()
        {
            try
            {
                while (true)
                {
                    var (request, taskCompletionSource) = await RequestReader.ReadAsync();
                    var response = await Caller.PostAsync(request);
                    taskCompletionSource.SetResult(response);

                    //await ResponseWriter.WriteAsync(response);
                }
            }
            catch (ChannelClosedException)
            {
                Console.WriteLine("Channel closed");
            }
        }
    }
}
