using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace AsynchronousTraining
{
    public class CallConsumer
    {
        private readonly IHttpCallable Caller;

        public CallConsumer(IHttpCallable caller, int requestLimit)
        {
            Caller = caller;
            RequestLimit = requestLimit;
        }

        public ChannelReader<(Request, TaskCompletionSource<Response>)> RequestReader { get; set; }

        public int RequestLimit { get; }


        public async Task StartConsumeAsync()
        {
            while (true)
            {
                var (request, taskCompletionSource) = await RequestReader.ReadAsync();
                var response = await Caller.PostAsync(request);
                taskCompletionSource.SetResult(response);
            }
        }
    }
}
