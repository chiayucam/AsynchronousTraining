using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AsynchronousTraining
{
    public class CustomReportCaller
    {
        private HttpClient Client;

        private string BaseUri;

        public CustomReportCaller(string baseUri, HttpClient client)
        {
            Client = client;
            BaseUri = baseUri;
        }

        public async Task<CustomReportResponse> PostAsync(CustomReportRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await Client.PostAsync(BaseUri, stringContent);
            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CustomReportResponse>(result);
        }
    }
}
