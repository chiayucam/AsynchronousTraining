using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousTraining
{
    public interface IHttpCallable
    {
        Task<Response> PostAsync(Request request);
    }
}
