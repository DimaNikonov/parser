using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLog
{
    public class LogInfo
    {
        public int ID { get; set; }
        public string IpAddress { get; set; }
        public DateTime Date { get; set; }
        public string HttpRequestMethod { get; set; }
        public int  HttpStatusCode { get; set; }
        public string  Path { get; set; }
        public int Size { get; set; }
    }
}
