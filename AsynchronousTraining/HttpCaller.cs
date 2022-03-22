using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AsynchronousTraining
{
    public class HttpCaller
    {
        private static HttpClient Client = new HttpClient();

        private int Dtno;

        private int Ftno;

        private string Params;

        private string KeyMap;

        private string AssignSpid;

        public HttpCaller(int dtno, int ftno, string _params, string keyMap, string assignSpid)
        {
            Dtno = dtno;
            Ftno = ftno;
            Params = _params;
            KeyMap = keyMap;
            AssignSpid = assignSpid;
        }

        public async Task<string> PostAsync()
        {
            string json = JsonConvert.SerializeObject(new
            {
                dtno = Dtno,
                ftno = Ftno,
                _params = Params,
                keyMap = KeyMap,
                assignSpid = AssignSpid
            });

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            // 呼叫並取出結果
            string baseUri = "http://192.168.10.146:5000/api/customreport/";
            HttpResponseMessage response = await Client.PostAsync(baseUri, stringContent);
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
