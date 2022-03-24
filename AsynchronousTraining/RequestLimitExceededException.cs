using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousTraining
{
    public class RequestLimitExceededException : Exception
    {
        public RequestLimitExceededException()
        {
        }

        public RequestLimitExceededException(string message) : base(message)
        {
        }
    }
}