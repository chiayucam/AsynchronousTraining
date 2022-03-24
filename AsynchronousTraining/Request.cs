using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousTraining
{
    public class Request
    {
        public int Dtno { get; set; }

        public int Ftno { get; set; }

        public string @Params { get; set; }

        public string KeyMap { get; set; }

        public string AssignSpid { get; set; }
    }
}
