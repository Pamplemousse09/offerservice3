using System;
using System.Collections.Generic;

namespace Kikai.Logging.DTO
{
    public class LogObject
    {
        public DateTime TimeStamp { get; set; }

        public string RequestId { get; set; }

        public string User { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public Dictionary<string, string> Response { get; set; }
    }
}
