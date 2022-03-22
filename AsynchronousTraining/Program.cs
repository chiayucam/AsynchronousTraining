using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsynchronousTraining
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            int dtno = 5493;
            int ftno = 0;
            string _params = "AssignID=00878;AssignDate=20200101-99999999;DTOrder=2;MTPeriod=2;";
            string keyMap = "";
            string assignSpid = "";

            var httpCaller = new HttpCaller(dtno, ftno, _params, keyMap, assignSpid);
            var result = await httpCaller.PostAsync();
            Console.WriteLine(result);
        }
    }
}
