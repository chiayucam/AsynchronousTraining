using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousTraining
{
    public interface ICustomReportCaller
    {
        Task<Response> PostAsync(Request request);
    }
}
