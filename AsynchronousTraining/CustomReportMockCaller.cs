using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AsynchronousTraining
{
    public class CustomReportMockCaller
    {
        private HttpClient Client;

        private string BaseUri;

        private int ResponseTime;

        public static int MaxConcurrentRequest { get; set; }

        public static int CurrentConcurrentRequest { get; private set; }

        public CustomReportMockCaller(string baseUri, HttpClient client, int responseTime)
        {
            Client = client;
            BaseUri = baseUri;
            ResponseTime = responseTime;
        }

        public async Task<CustomReportResponse> PostAsync(CustomReportRequest request)
        {
            while(CurrentConcurrentRequest <= MaxConcurrentRequest)
            {
                CurrentConcurrentRequest++;

                var json = JsonConvert.SerializeObject(request);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                await MockRequest();
            }

            var response = new CustomReportResponse();
            return response;
        }

        private Task<CustomReportResponse> MockRequest()
        {
            Thread.Sleep(ResponseTime);
            var response = new CustomReportResponse();
            return Task.FromResult(response);
        }

    }
}
