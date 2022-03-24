using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousTraining
{
    public class Response
    {
        public bool IsCompleted { get; set; }

        public bool IsFaulted { get; set; }

        public string Signature { get; set; }

        public string Expception { get; set; }

        public string Result { get; set; }
    }
}
