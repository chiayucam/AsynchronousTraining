using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AsynchronousTraining
{
    /// <summary>
    /// 自訂報表呼叫
    /// </summary>
    public class CustomReportCaller : IHttpCallable
    {
        /// <summary>
        /// HttpClient
        /// </summary>
        private HttpClient Client;

        /// <summary>
        /// BaseUri
        /// </summary>
        private string BaseUri;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="baseUri">baseUri</param>
        /// <param name="client">HttpClient</param>
        public CustomReportCaller(string baseUri, HttpClient client)
        {
            Client = client;
            BaseUri = baseUri;
        }

        /// <summary>
        /// 呼叫API
        /// </summary>
        /// <param name="request">Request類別的request body</param>
        /// <returns>Response類別的response body</returns>
        public async Task<Response> PostAsync(Request request)
        {
            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await Client.PostAsync(BaseUri, stringContent);
            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Response>(result);
        }
    }
}
